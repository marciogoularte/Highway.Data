﻿
using System;


namespace Highway.Data.Test.InMemory.Domain
{
	public class Post
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }

		public Blog Blog { get; set; }
	}
}