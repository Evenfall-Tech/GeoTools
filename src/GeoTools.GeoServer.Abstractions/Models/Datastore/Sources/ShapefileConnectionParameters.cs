using GeoTools.GeoServer.Models.Catalog;
using System;
using System.Collections.Generic;

namespace GeoTools.GeoServer.Models.Datastore.Sources
{
    public class ShapefileConnectionParameters : ConnectionParameters
    {
        public ShapefileConnectionParameters(Uri shapefilePath, Uri @namespace) : base(CreateConnectionParameters(shapefilePath, @namespace))
        {

        }

        private static IList<ConnectionParameter> CreateConnectionParameters(Uri shapefilePath, Uri @namespace)
        {
            return new List<ConnectionParameter>
            {
                new ConnectionParameter("namespace", @namespace.ToString()),
                new ConnectionParameter("url", shapefilePath.ToString()),
            };
        }
    }
}
