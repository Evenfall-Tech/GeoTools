﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models
{
    public class WorkspaceResponseWrapper
    {
        [JsonPropertyName("workspace")]
        public IList<NamedLink> Workspace { get; }

        [JsonConstructor]
        public WorkspaceResponseWrapper(IList<NamedLink> workspace)
        {
            Workspace = workspace;
        }
    }
}
