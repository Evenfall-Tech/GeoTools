using GeoTools.GeoServer.Resources;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace GeoTools.GeoServer.Helpers.Options
{
    internal class ValidateGeoServerOptions : IValidateOptions<GeoServerOptions>
    {
        public ValidateOptionsResult Validate(string name, GeoServerOptions options)
        {
            var failures = new List<string>();

            if (options is null)
                return ValidateOptionsResult.Fail(Messages.Options_Null);

            if (options.ConfigureHttpClient != null)
                return ValidateOptionsResult.Success;

            if (options.BaseAddress == null)
                failures.Add(Messages.Options_BaseAddressNull);
            else if (options.BaseAddress.IsFile)
                failures.Add(Messages.Options_BaseAddressFile);

            if (options.AuthorizationHeaderValue != null &&
                options.AuthorizationHeaderValue.Split(
                    new char[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries).Length < 1)
                failures.Add(Messages.Options_AuthorizationHeaderValueWrongFormat);

            if (failures.Count > 0)
                return ValidateOptionsResult.Fail(failures);
            else
                return ValidateOptionsResult.Success;
        }
    }
}
