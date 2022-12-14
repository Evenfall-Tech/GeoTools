﻿using System.Text.Json.Serialization;

namespace GeoTools.GeoServer.Models
{
    internal class GetWorkspaceResponse
    {
        [JsonPropertyName("workspace")]
        public WorkspaceSummary Workspace { get; }

        [JsonConstructor]
        public GetWorkspaceResponse(WorkspaceSummary workspace)
        {
            Workspace = workspace;
        }
    }
}