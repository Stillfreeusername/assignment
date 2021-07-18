using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Helpers
{
	public static class BackendSettings
	{
		public static readonly System.Text.Json.JsonSerializerOptions DefaultMicrosoftJsonFormatter = new System.Text.Json.JsonSerializerOptions();

		public static readonly Newtonsoft.Json.JsonSerializerSettings DefaultNewtonsoftJsonFormatter = new Newtonsoft.Json.JsonSerializerSettings();

		static BackendSettings()
		{
			DefaultNewtonsoftJsonFormatter.CheckAdditionalContent = false;
			DefaultNewtonsoftJsonFormatter.Culture = System.Globalization.CultureInfo.InvariantCulture;
			DefaultNewtonsoftJsonFormatter.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
			DefaultNewtonsoftJsonFormatter.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
			DefaultNewtonsoftJsonFormatter.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
			DefaultNewtonsoftJsonFormatter.FloatFormatHandling = Newtonsoft.Json.FloatFormatHandling.String;
			DefaultNewtonsoftJsonFormatter.FloatParseHandling = Newtonsoft.Json.FloatParseHandling.Double;
			DefaultNewtonsoftJsonFormatter.Formatting = Newtonsoft.Json.Formatting.None;
			DefaultNewtonsoftJsonFormatter.MaxDepth = 64;
			DefaultNewtonsoftJsonFormatter.MetadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.Default;
			DefaultNewtonsoftJsonFormatter.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
			DefaultNewtonsoftJsonFormatter.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
			DefaultNewtonsoftJsonFormatter.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto;
			DefaultNewtonsoftJsonFormatter.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
			DefaultNewtonsoftJsonFormatter.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
			DefaultNewtonsoftJsonFormatter.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default;
			DefaultNewtonsoftJsonFormatter.TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple;
			DefaultNewtonsoftJsonFormatter.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;

			DefaultMicrosoftJsonFormatter.AllowTrailingCommas = true;
			DefaultMicrosoftJsonFormatter.IgnoreNullValues = true;
			DefaultMicrosoftJsonFormatter.IgnoreReadOnlyProperties = true;
			DefaultMicrosoftJsonFormatter.MaxDepth = 64;
			DefaultMicrosoftJsonFormatter.PropertyNameCaseInsensitive = true;
			DefaultMicrosoftJsonFormatter.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
			DefaultMicrosoftJsonFormatter.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
			DefaultMicrosoftJsonFormatter.WriteIndented = false;
			DefaultMicrosoftJsonFormatter.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
		}
	}
}