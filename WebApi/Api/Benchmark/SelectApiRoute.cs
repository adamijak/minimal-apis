using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Objects;

namespace WebApi.Api.Benchmark;

public class SelectBenchmark : IApiRoute
{
    public void Map(RouteGroupBuilder group)
    {
        group.Map("benchmark/select", OnGet);
    }

    public object OnGet([FromQuery] string[] select)
    {
        var users = new UsersApiRoute().OnGet();
        var stopwatch = new Stopwatch();

        stopwatch.Start();
        var result1 = users.Select(QueryParameterHelpers.GetSelectFunction<User>(select));
        stopwatch.Stop();
        var time1 = stopwatch.ElapsedTicks;

        stopwatch.Start();
        var result2 = users.Select(QueryParameterHelpers.GetCompiledSelectFunction<User>(select));
        stopwatch.Stop();
        var time2 = stopwatch.ElapsedTicks;

        return new { result1, time1, result2, time2, };
    }
}

