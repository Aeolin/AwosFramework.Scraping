using AwosFramework.Scraping.ResultHandling.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AwosFramework.Scraping.Hosting.ResultHandlers;

public partial class JsonResultHandlerBuilder<T>
{
	private Predicate<T> _filter;
	private string _directory;
	private string _fileNameTemplate;
	private int _batchSize;
	private JsonSerializerOptions _options;
	private readonly List<JsonResultHandlerCategoryBuilder> _categories = new();

	public JsonResultHandlerBuilder<T> FromConfig(IConfiguration config)
	{
		_directory = config.GetValue("Directory", _directory);
		_fileNameTemplate = config.GetValue("FileNameTemplate", _fileNameTemplate);
		_batchSize = config.GetValue("BatchSize", _batchSize);
		return this;
	}

	public JsonResultHandlerBuilder<T> WithFilter(Predicate<T> filter)
	{
		_filter = filter;
		return this;
	}

	public JsonResultHandlerBuilder<T> WithCategory(string name, Predicate<T> filter, Action<JsonResultHandlerCategoryBuilder> catBuilder = null)
	{
		var categoryBuilder = new JsonResultHandlerCategoryBuilder(this, name, filter);
		catBuilder?.Invoke(categoryBuilder);
		_categories.Add(categoryBuilder);
		return this;
	}
 
	public JsonResultHandlerBuilder<T> WithDirectory(string directory)
	{
		_directory = directory;
		return this;
	}

	public JsonResultHandlerBuilder<T> WithFileNameTemplate(string fileNameTemplate)
	{
		_fileNameTemplate = fileNameTemplate;
		return this;
	}

	public JsonResultHandlerBuilder<T> WithBatchSize(int batchSize)
	{
		_batchSize = batchSize;
		return this;
	}

	public JsonResultHandlerBuilder<T> WithSerializerOptions(JsonSerializerOptions options)
	{
		_options = options;
		return this;
	}

	public JsonResultHandler<T> Build()
	{
		if (_categories.Count == 0)
			WithCategory("Default", _filter);

		var categories = _categories.Select(x => x.Build());
		return new JsonResultHandler<T>(categories, _filter);
	}
}
