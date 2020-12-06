using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace ReactUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Wrong place: ReactUI gets run by npm!");
        }
    }
}
