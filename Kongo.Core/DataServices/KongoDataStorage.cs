using Microsoft.EntityFrameworkCore;
using Kongo.Core.Models;

namespace Kongo.Core.DataServices
{
	public class KongoDataStorage : DbContext
	{
		private SqliteConfigurationModel _config;

		public KongoDataStorage(SqliteConfigurationModel config) {
			_config = config;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite($"Data Source={_config.DatabaseName}");
		}

		public DbSet<LogIngestionModel> Logs { get; set; }
		public DbSet<NodeStatisticsModel> NodeStats { get; set; }
		public DbSet<StoredFragmentsModel> Fragments { get; set; }
		public DbSet<ProcessedNetworkStatisticsModel> NetworkStats { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//Define the Table(s) and References to be created automatically
			modelBuilder.Entity<LogIngestionModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Msg).IsRequired().HasMaxLength(255);
				b.Property(e => e.Level).IsRequired().HasMaxLength(10);
				b.Property(e => e.Ts).IsRequired();
				b.Property(e => e.Node_id).HasMaxLength(64);
				b.Property(e => e.Peer_addr).IsRequired().HasMaxLength(25);
				b.Property(e => e.Task).IsRequired().HasMaxLength(30);
				b.Property(e => e.Reason).IsRequired(false).HasMaxLength(255);
				b.ToTable("LogEntries");
			});

			modelBuilder.Entity<NodeStatisticsModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Timestamp).IsRequired(true);
				b.Property(e => e.BlockRecvCnt);
				b.Property(e => e.LastBlockDate).HasMaxLength(20);
				b.Property(e => e.LastBlockFees);
				b.Property(e => e.LastBlockHash).HasMaxLength(64);
				b.Property(e => e.LastBlockHeight).HasMaxLength(20);
				b.Property(e => e.lastBlockSum);
				b.Property(e => e.LastBlockTime).IsRequired(false);
				b.Property(e => e.LastBlockTx);
				b.Property(e => e.TxRecvCnt);
				b.Property(e => e.Uptime);
				b.ToTable("NodeStatisticEntries");
			});

			modelBuilder.Entity<StoredFragmentsModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Timestamp);
				b.Property(e => e.FragmentsReceviedFromRest);
				b.Property(e => e.FragmentsReceviedFromNetwork);
				b.Property(e => e.FragmentsInBlock);
				b.Property(e => e.FragmentsPending);
				b.Property(e => e.TotalFragments);
				b.ToTable("FragmentStatistics");
			});

			modelBuilder.Entity<ProcessedNetworkStatisticsModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Timestamp);
				b.Property(e => e.TotalEstablishedConnections);
				b.Property(e => e.BlocksReceivedInPast30Min);
				b.Property(e => e.FragmentsReceivedInPast30Min);
				b.Property(e => e.GossipReceivedInPast30Min);
				b.Property(e => e.LastBlockReceivedAt);
				b.Property(e => e.LastFragmentReceivedAt);
				b.Property(e => e.LastGossipReceivedAt);
				b.ToTable("NetworkStatistics");
			});

		}
	}
}
