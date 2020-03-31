﻿using System;

namespace Perplex.ContentBlocks.Categories
{
    public class SerializableContentBlockCategory : IContentBlockCategory
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public bool IsHidden { get; set; }
    }
}
