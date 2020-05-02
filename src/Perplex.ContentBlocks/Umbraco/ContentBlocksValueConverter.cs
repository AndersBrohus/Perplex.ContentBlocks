﻿using Newtonsoft.Json;
using Perplex.ContentBlocks.Definitions;
using Perplex.ContentBlocks.Rendering;
using Perplex.ContentBlocks.Umbraco.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;
using Umbraco.Web.PropertyEditors.ValueConverters;

namespace Perplex.ContentBlocks.Umbraco
{
    public class ContentBlocksValueConverter : PropertyValueConverterBase
    {
        private readonly NestedContentSingleValueConverter _nestedContentSingleValueConverter;

        public ContentBlocksValueConverter(NestedContentSingleValueConverter nestedContentSingleValueConverter)
        {
            _nestedContentSingleValueConverter = nestedContentSingleValueConverter;
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        {
            return PropertyCacheLevel.Snapshot;
        }

        public override bool IsConverter(IPublishedPropertyType propertyType)
            => propertyType.EditorAlias == Constants.Umbraco.ContentBlocksPropertyEditorAlias;

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            string json = inter?.ToString();
            if (string.IsNullOrEmpty(json))
            {
                return Rendering.ContentBlocks.Empty;
            }

            ContentBlocksModelValue modelValue;

            try
            {
                modelValue = JsonConvert.DeserializeObject<ContentBlocksModelValue>(json);
            }
            catch
            {
                return Rendering.ContentBlocks.Empty;
            }

            IContentBlockViewModel createViewModel(ContentBlockModelValue block)
            {
                if (block == null || block.IsDisabled)
                {
                    return null;
                }

                IContentBlockDefinitionRepository definitionRepository = Current.Factory.GetInstance<IContentBlockDefinitionRepository>();
                if (definitionRepository == null)
                {
                    return null;
                }

                IContentBlockDefinition definition = definitionRepository.GetById(block.DefinitionId);
                if (definition == null || definition.Layouts == null || definition.Layouts.Any() == false)
                {
                    return null;
                }

                IContentBlockLayout layout = definition.Layouts.FirstOrDefault(l => l.Id == block.LayoutId);
                if (layout == null)
                {
                    return null;
                }

                IPublishedElement content = _nestedContentSingleValueConverter.ConvertIntermediateToObject(owner, propertyType, referenceCacheLevel, block?.Content?.ToString(), preview) as IPublishedElement;
                if (content == null)
                {
                    return null;
                }

                var contentType = content.GetType();
                var genericViewModelFactoryType = typeof(IContentBlockViewModelFactory<>).MakeGenericType(new[] { contentType });
                var viewModelFactory = Current.Factory.GetInstance(genericViewModelFactoryType) as IContentBlockViewModelFactory;

                if (viewModelFactory == null)
                {
                    return null;
                }

                return viewModelFactory.Create(content, block.Id, block.DefinitionId, block.LayoutId);
            }

            EditorLayout editorLayout = GetEditorLayout(propertyType);

            return new Rendering.ContentBlocks
            {
                Header = editorLayout == EditorLayout.Blocks ? null : createViewModel(modelValue.Header),
                Blocks = editorLayout == EditorLayout.Header
                    ? Enumerable.Empty<IContentBlockViewModel>()
                    : modelValue.Blocks
                        .Select(createViewModel)
                        .Where(rm => rm != null)
                        .ToList()
            };
        }

        private EditorLayout GetEditorLayout(IPublishedPropertyType propertyType)
        {
            if (propertyType.DataType.Configuration is IDictionary<string, object> config &&
                config.TryGetValue(Constants.Umbraco.Configuration.EditorLayoutKey, out object configuredLayout) &&
                Enum.TryParse(configuredLayout.ToString(), true, out EditorLayout layout))
            {
                return layout;
            };

            // Default
            return EditorLayout.All;
        }

        public override Type GetPropertyValueType(IPublishedPropertyType propertyType)
            => typeof(IContentBlocks);
    }
}
