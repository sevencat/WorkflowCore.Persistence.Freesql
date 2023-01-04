using Newtonsoft.Json;

namespace WorkflowCore.Persistence.Freesql;

public class ExtensionMethods
{
	private static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
}