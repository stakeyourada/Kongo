﻿using Kongo.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace Kongo.Core.DataServices
{
	public class KongoDataStorage : DbContext
	{
		string _connectionString;

		// The constructor that ASP.NET Core expects. LINQPad can use it too.
		public KongoDataStorage(DbContextOptions<KongoDataStorage> options) : base(options) { }

		// This constructor is simpler and more robust. Use it if LINQPad errors on the constructor above.
		// Note that _connectionString is picked up in the OnConfiguring method below.
		public KongoDataStorage(string connectionString) => _connectionString = connectionString;

		// This constructor obtains the connection string from your appsettings.json file.
		// Tell LINQPad to use it if you don't want to specify a connection string in LINQPad's dialog.
		public KongoDataStorage()
		{
			IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			_connectionString = config.GetConnectionString("DefaultConnection");
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
			IConfigurationRoot config = builder.Build();

			// Assign _connectionString to the optionsBuilder:
			if (_connectionString != null)
				optionsBuilder.UseSqlite(_connectionString);  
			else
				optionsBuilder.UseSqlite(config.GetConnectionString("KongoDb"));

			// Recommended: uncomment the following line to enable lazy-loading navigation hyperlinks in LINQPad:
			if (InsideLINQPad) optionsBuilder.UseLazyLoadingProxies();
			// (You'll need to add a reference to the Microsoft.EntityFrameworkCore.Proxies NuGet package, and
			//  mark your navigation properties as virtual.)

			// Recommended: uncomment the following line to enable the SQL trace window:
			if (InsideLINQPad) optionsBuilder.EnableSensitiveDataLogging (true);
		}

		public DbSet<LogIngestionModel> LogEntries { get; set; }
		public DbSet<NodeStatisticsModel> NodeStatisticEntries { get; set; }
		public DbSet<StoredFragmentsModel> FragmentStatistics { get; set; }
		public DbSet<ProcessedNetworkStatisticsModel> NetworkStatistics { get; set; }
		public DbSet<ProcessedLeadersLogsModel> LeadersLogs { get; set; }
		public DbSet<ProcessedStakeModel> StakeDistribution { get; set; }
		public DbSet<StoredSettingsModel> Settings { get; set; }

		public bool InsideLINQPad { get; private set; }

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
				b.HasIndex(b => b.Ts);
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
				b.HasIndex(b => b.Timestamp);
				b.Property(e => e.BlockRecvCnt);
				b.Property(e => e.LastBlockDate).IsRequired(true).HasMaxLength(20);
				b.Property(e => e.LastBlockFees);
				b.Property(e => e.LastBlockHash).HasMaxLength(64);
				b.Property(e => e.LastBlockHeight).HasMaxLength(20);
				b.Property(e => e.lastBlockSum);
				b.Property(e => e.LastBlockTime).IsRequired(false);
				b.Property(e => e.LastBlockTx);
				b.Property(e => e.TxRecvCnt);
				b.Property(e => e.State).HasMaxLength(20);
				b.Property(e => e.Uptime);
				b.ToTable("NodeStatisticEntries");
			});

			modelBuilder.Entity<StoredFragmentsModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Timestamp);
				b.HasIndex(b => b.Timestamp);
				b.Property(e => e.FragmentsReceviedFromRest);
				b.Property(e => e.FragmentsReceviedFromNetwork);
				b.Property(e => e.FragmentsInBlock);
				b.Property(e => e.FragmentsRejected);
				b.Property(e => e.FragmentsPending);
				b.Property(e => e.TotalFragments);
				b.ToTable("FragmentStatistics");
			});

			modelBuilder.Entity<ProcessedNetworkStatisticsModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Timestamp);
				b.HasIndex(b => b.Timestamp);
				b.Property(e => e.TotalEstablishedConnections);
				b.Property(e => e.BlocksReceivedInPast30Min);
				b.Property(e => e.FragmentsReceivedInPast30Min);
				b.Property(e => e.GossipReceivedInPast30Min);
				b.Property(e => e.LastBlockReceivedAt);
				b.Property(e => e.LastFragmentReceivedAt);
				b.Property(e => e.LastGossipReceivedAt);
				b.ToTable("NetworkStatistics");
			});

			modelBuilder.Entity<ProcessedLeadersLogsModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Timestamp);
				b.HasIndex(b => b.Timestamp);
				b.Property(e => e.LeadersLogsJson);
				b.ToTable("LeadersLogs");
			});

			modelBuilder.Entity<ProcessedStakeModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Timestamp);
				b.HasIndex(b => b.Timestamp);
				b.Property(e => e.Epoch);
				b.Property(e => e.Dangling);
				b.Property(e => e.PoolDistributionJson);
				b.Property(e => e.Unassigned);
				b.ToTable("StakeDistribution");
			});

			modelBuilder.Entity<StoredSettingsModel>(b =>
			{
				b.HasKey(e => e.Id);
				b.Property(e => e.Id).ValueGeneratedOnAdd();
				b.Property(e => e.Timestamp);
				b.HasIndex(b => b.Timestamp);
				b.Property(e => e.Block0Hash);
				b.Property(e => e.ConsensusVersion);
				b.Property(e => e.CurrSlotStartTime);
				b.Property(e => e.Certificate);
				b.Property(e => e.Coefficient);
				b.Property(e => e.Constant);
				b.Property(e => e.MaxTxsPerBlock);
				b.Property(e => e.SlotDuration);
				b.Property(e => e.SlotsPerEpoch);
				b.ToTable("Settings");
			});

		}
	}
}
