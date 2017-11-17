﻿using Highway.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Highway.RestArea
{
	public class Handler<TContext, TEntity, TId, TModel>
		where TContext : UnitOfWork
		where TId : IEquatable<TId>
		where TEntity : class, IIdentifiable<TId>

	{
		private readonly RestAreaOptions<TContext> restAreaOptions;
		private readonly EntityOptions<TEntity, TId, TModel> options;
		private readonly IRepository repo;
		private readonly ILogger logger;
		public Handler(IRepository repo, RestAreaOptions<TContext> restAreaOptions, EntityOptions<TEntity, TId, TModel> options, ILogger logger)
		{
			this.restAreaOptions = restAreaOptions;
			this.options = options;
			this.logger = logger;
			this.repo = repo;
		}

		public async Task GetAll(HttpContext context, RouteData routeData)
		{
			var data = (await repo.FindAsync(options.GetAllFactory())).ToList();
			Json<List<TModel>>(data, context);
		}

		public async Task GetOne(HttpContext context, RouteData routeData)
		{
			TId id = restAreaOptions.ConvertTo<TId>(routeData.Values[options.IdentityRouteValue].ToString());
			var data = await repo.FindAsync(options.GetByIdFactory(id));
			if (data == null) PageNotFound(context);
			else Json<TModel>(data, context);
		}

		private void PageNotFound(HttpContext context)
		{
			context.Response.StatusCode = 404;
		}

		private void Json<TReturn>(object obj, HttpContext context)
		{
			context.Response.StatusCode = 200;
			context.Response.ContentType = "application/json";
			using (var writer = new StreamWriter(context.Response.Body))
			{
				TReturn model = restAreaOptions.GetMapper().Map<TReturn>(obj);
				restAreaOptions.Serializer.Serialize(writer, model);
			}
		}
	}
}
