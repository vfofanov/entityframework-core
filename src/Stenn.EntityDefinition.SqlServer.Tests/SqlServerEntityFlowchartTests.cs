#nullable enable
using System;
using System.Drawing;
using NUnit.Framework;
using Stenn.EntityDefinition.EntityFrameworkCore;
using Stenn.EntityDefinition.EntityFrameworkCore.Flowchart;
using Stenn.EntityDefinition.Flowchart;
using Stenn.EntityDefinition.Model.Definitions;
using Stenn.Shared.Mermaid;

// ReSharper disable UnusedVariable

namespace Stenn.EntityDefinition.SqlServer.Tests
{
    public class SqlServerEntityFlowchartTests : SqlServerEntityTestsBase
    {
        [Test]
        public void TestFlowchart()
        {
            var options = GetFlowchartGraphBuilderOptions();

            var graphBuilder = new EFFlowchartGraphBuilder(options);
            var outputEditor = graphBuilder.Build(_dbContext).ToString(MermaidPrintConfig.Normal);
            var outputHtml = graphBuilder.Build(_dbContext).ToString(MermaidPrintConfig.ForHtml);
        }

        private static EFFlowchartGraphBuilderOptions GetFlowchartGraphBuilderOptions()
        {
            var options = new EFFlowchartGraphBuilderOptions();
            options.InitReaderOptions(opt => { opt.AddEntityColumn(CustomDefinitions.Domain); });

            options.Property.DrawAsNode = false;
            options.DrawRelationAsNode = false;

            options.Entity.AddInheritRelations = false;
            options.CleanNodesWithoutRelations = true;

            //Uncomment for short ralation caption
            //options.Property.RelationCaption = EFCommonDefinitions.Properties.Navigation.ShortRelationCaption;

            //Uncomment for cross domain relations only
            //options.Property.SetFilter(propertyRow => propertyRow.GetValueOrDefault(CustomDefinitions.IsDomainDifferent.Info));

            options.Entity.AddGraphGroup(CustomDefinitions.Domain.ToFlowchartGraphGroup((domain, styleClass) =>
                {
                    var color = domain switch
                    {
                        Domain.Unknown => Color.LightGray,
                        Domain.Security => Color.PaleVioletRed,
                        Domain.Order => Color.Aquamarine,
                        _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, null)
                    };

                    styleClass.SetModifier("fill", ColorTranslator.ToHtml(color))
                        .SetModifier("stroke-width", "2px")
                        .SetModifier("stroke-dasharray", "2 2");
                },
                extractCaption: domain => $"Domain:{domain}",
                skipDuringClean: true)
            );

            return options;
        }
    }
}