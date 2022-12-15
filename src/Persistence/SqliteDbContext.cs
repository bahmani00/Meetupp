using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence;

public class SqliteDbContext : AppDbContext, IAppDbContext {

  public SqliteDbContext(
    DbContextOptions options,
    AuditEntitySaveChangesInterceptor auditInterceptor,
    IConfiguration configuration)
    : base(options, auditInterceptor, configuration) {
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    optionsBuilder.UseSqlite(
      configuration.GetConnectionString("SqliteConnection"),
      x => x.MigrationsAssembly(typeof(SqliteDbContext).Assembly.FullName));
    optionsBuilder.AddInterceptors(auditEntitySaveChangesInterceptor);
  }
}