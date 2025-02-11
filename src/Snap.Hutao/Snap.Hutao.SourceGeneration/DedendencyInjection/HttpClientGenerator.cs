﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Snap.Hutao.SourceGeneration.DedendencyInjection;

/// <summary>
/// 注入HttpClient代码生成器
/// 旨在使用源生成器提高注入效率
/// 防止在运行时动态查找注入类型
/// </summary>
[Generator]
public class HttpClientGenerator : ISourceGenerator
{
    private const string DefaultName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfigration.Default";
    private const string XRpcName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.HttpClientConfigration.XRpc";

    private const string PrimaryHttpMessageHandlerAttributeName = "Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient.PrimaryHttpMessageHandlerAttribute";

    /// <inheritdoc/>
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new HttpClientSyntaxContextReceiver());
    }

    /// <inheritdoc/>
    public void Execute(GeneratorExecutionContext context)
    {
        // retrieve the populated receiver
        if (context.SyntaxContextReceiver is not HttpClientSyntaxContextReceiver receiver)
        {
            return;
        }

        string toolName = this.GetGeneratorType().FullName;

        StringBuilder sourceCodeBuilder = new();
        sourceCodeBuilder.Append($@"// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

// This class is generated by Snap.Hutao.SourceGeneration

using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace Snap.Hutao.Core.DependencyInjection;

internal static partial class IocHttpClientConfiguration
{{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(""{toolName}"",""1.0.0.0"")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    public static partial IServiceCollection AddHttpClients(this IServiceCollection services)
    {{");

        FillWithInjectionServices(receiver, sourceCodeBuilder);
        sourceCodeBuilder.Append(@"
        return services;
    }
}");

        context.AddSource("IocHttpClientConfiguration.g.cs", SourceText.From(sourceCodeBuilder.ToString(), Encoding.UTF8));
    }

    private static void FillWithInjectionServices(HttpClientSyntaxContextReceiver receiver, StringBuilder sourceCodeBuilder)
    {
        List<string> lines = new();
        StringBuilder lineBuilder = new();

        foreach (INamedTypeSymbol classSymbol in receiver.Classes)
        {
            lineBuilder
                .Clear()
                .Append("\r\n");

            lineBuilder.Append(@"        services.AddHttpClient<");
            lineBuilder.Append($"{classSymbol.ToDisplayString()}>(");

            AttributeData httpClientInfo = classSymbol
                .GetAttributes()
                .Single(attr => attr.AttributeClass!.ToDisplayString() == HttpClientSyntaxContextReceiver.AttributeName);
            ImmutableArray<TypedConstant> arguments = httpClientInfo.ConstructorArguments;

            TypedConstant injectAs = arguments[0];

            string injectAsName = injectAs.ToCSharpString();
            switch (injectAsName)
            {
                case DefaultName:
                    lineBuilder.Append(@"DefaultConfiguration)");
                    break;
                case XRpcName:
                    lineBuilder.Append(@"XRpcConfiguration)");
                    break;
                default:
                    throw new InvalidOperationException($"非法的HttpClientConfigration值: [{injectAsName}]");
            }

            AttributeData? handlerInfo = classSymbol
                .GetAttributes()
                .SingleOrDefault(attr => attr.AttributeClass!.ToDisplayString() == PrimaryHttpMessageHandlerAttributeName);

            if (handlerInfo != null)
            {
                ImmutableArray<KeyValuePair<string, TypedConstant>> properties = handlerInfo.NamedArguments;
                lineBuilder.Append(@".ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler() {");

                foreach (KeyValuePair<string, TypedConstant> property in properties)
                {
                    lineBuilder.Append(" ");
                    lineBuilder.Append(property.Key);
                    lineBuilder.Append(" = ");
                    lineBuilder.Append(property.Value.ToCSharpString());
                    lineBuilder.Append(",");
                }

                lineBuilder.Append(" })");
            }

            lineBuilder.Append(";");

            lines.Add(lineBuilder.ToString());
        }

        foreach (string line in lines.OrderBy(x => x))
        {
            sourceCodeBuilder.Append(line);
        }
    }
}