﻿using Npgsql;
using TrappyKeepy.Domain.Interfaces;
using TrappyKeepy.Domain.Maps;
using TrappyKeepy.Domain.Models;

namespace TrappyKeepy.Data.Repositories
{
    public class KeeperRepository : BaseRepository, IKeeperRepository
    {
        public KeeperRepository(NpgsqlConnection connection) : base(connection)
        {
            _connection = connection;
        }

        public async Task<Keeper> Create(Keeper keeper)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_create('{keeper.Filename}', '{keeper.ContentType}', '{keeper.UserPosted}'";

                if (keeper.Description is not null) command.CommandText += $", '{keeper.Description}'";
                else command.CommandText += $", null";

                if (keeper.Category is not null) command.CommandText += $", '{keeper.Category}'";
                else command.CommandText += $", null";

                command.CommandText += $");";

                var reader = await RunQuery(command);

                var newKeeper = new Keeper();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    newKeeper = map.Keeper(reader);
                }
                reader.Close();
                return newKeeper;
            }
        }

        public async Task<List<Keeper>> ReadAll()
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_read_all();";

                var reader = await RunQuery(command);
                var keepers = new List<Keeper>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    keepers.Add(map.Keeper(reader));
                }
                reader.Close();
                return keepers;
            }
        }

        public async Task<Keeper> ReadById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_read_by_id('{id}');";

                var reader = await RunQuery(command);
                var keeper = new Keeper();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    keeper = map.Keeper(reader);
                }
                reader.Close();
                return keeper;
            }
        }

        public async Task<List<Keeper>> ReadAllPermitted(Guid requestingUserId)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_read_all_permitted('{requestingUserId}');";

                var reader = await RunQuery(command);
                var keepers = new List<Keeper>();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    keepers.Add(map.Keeper(reader));
                }
                reader.Close();
                return keepers;
            }
        }

        public async Task<Keeper> ReadByIdPermitted(Guid keeperId, Guid requestingUserId)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_read_by_id_permitted('{keeperId}', '{requestingUserId}');";

                var reader = await RunQuery(command);
                var keeper = new Keeper();
                while (await reader.ReadAsync())
                {
                    var map = new PgsqlReaderMap();
                    keeper = map.Keeper(reader);
                }
                reader.Close();
                return keeper;
            }
        }

        public async Task<bool> UpdateById(Keeper keeper)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_update('{keeper.Id}'";

                if (keeper.Filename is not null) command.CommandText += $", '{keeper.Filename}'";
                else command.CommandText += $", null";

                if (keeper.Description is not null) command.CommandText += $", '{keeper.Description}'";
                else command.CommandText += $", null";

                if (keeper.Category is not null) command.CommandText += $", '{keeper.Category}'";
                else command.CommandText += $", null";

                command.CommandText += ");";

                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
            }
        }

        public async Task<bool> DeleteById(Guid id)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_delete_by_id('{id}');";

                var result = await RunScalar(command);
                var success = false;
                if (result is not null)
                {
                    success = bool.Parse($"{result.ToString()}");
                }
                return success;
            }
        }

        public async Task<int> CountByColumnValue(string column, string value)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = $"SELECT * FROM tk.keepers_count_by_column_value_text('{column}', '{value}');";

                var result = await RunScalar(command);
                int count = 0;
                if (result is not null)
                {
                    count = int.Parse($"{result.ToString()}");
                }
                return count;
            }
        }
    }
}
