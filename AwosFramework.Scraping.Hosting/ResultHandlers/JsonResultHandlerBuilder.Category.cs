using AwosFramework.Scraping.ResultHandling.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.ResultHandlers
{
	public partial class JsonResultHandlerBuilder<T>
	{
		public sealed class JsonResultHandlerCategoryBuilder
		{
			private readonly JsonResultHandlerBuilder<T> _parent;
			private string _fileNameTemplate;
			private readonly Predicate<T> _filter;
			private readonly string _name;
			private string _directory;
			private int? _batchSize;
			private JsonSerializerOptions _jsonOptions;

			public JsonResultHandlerCategoryBuilder WithFileNameTemplate(string fileNameTemplate)
			{
				_fileNameTemplate = fileNameTemplate;
				return this;
			}

			public JsonResultHandlerCategoryBuilder WithBatchSize(int batchSize)
			{
				_batchSize = batchSize;
				return this;
			}

			public JsonResultHandlerCategoryBuilder WithSerializerOptions(JsonSerializerOptions options)
			{
				_jsonOptions = options;
				return this;
			}

			public JsonResultHandlerCategoryBuilder WithDirectory(string directory)
			{
				_directory = directory;
				return this;
			}

			internal JsonResultHandlerCategoryBuilder(JsonResultHandlerBuilder<T> parent, string directory, Predicate<T> filter)
			{
				_parent = parent ?? throw new ArgumentNullException(nameof(parent));
				_name = directory;
				_filter = filter;
			}

			public JsonResultCategory<T> Build()
			{
				return new JsonResultCategory<T>(_name, _filter, _directory ?? _parent._directory, _jsonOptions ?? _parent._options, _batchSize ?? _parent._batchSize);
			}
		}
	}
}
