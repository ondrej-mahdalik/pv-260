using Microsoft.EntityFrameworkCore;
using PV260.API.DAL.Entities;

namespace PV260.API.DAL;

public class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
    public DbSet<EmailEntity> Emails { get; set; }
    public DbSet<ReportEntity> Reports { get; set; }
}