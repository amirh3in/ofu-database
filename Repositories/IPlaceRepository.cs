using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using reservationApi.Models;
using Dapper;
using Dapper.FastCrud;

namespace reservationApi.Repositories
{
    public interface IPlaceRepository
    {
        public List<Place> addPlace(Place place);
        public Place editPlace(PlaceEdit place);
        public string deletePlace(int id);
        public List<Place> searchByName(string s, int page);
        public List<Place> searchByType(string s, int page);
        public string reservePlace(Reserve reserve);
        public List<Place> searchAll(string name, string type, int page);
    }


    public class PlaceRepository : IPlaceRepository
    {
        private readonly IConfiguration _config;
        public PlaceRepository(IConfiguration config)
        {
            _config = config;
        }
        public List<Place> addPlace(Place place)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@name", place.name);
                parameters.Add("@address", place.address);
                parameters.Add("@type", place.type);
                parameters.Add("@addDate", DateTime.Now);
                parameters.Add("@authorId", place.authorId);

                try
                {
                    db.Query<Place>("insert into Places (name, address, type, addDate, authorId) values(@name, @address, @type, @addDate, @authorId)", parameters);
                }
                catch
                {
                    return null;
                }

                return db.Find<Place>().ToList();
            }
        }

        public string deletePlace(int id)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                try
                {
                    db.Delete<Place>(new Place { placeId = id });
                }
                catch
                {
                    return null;
                }

                return "success";
            }
        }

        public Place editPlace(PlaceEdit place)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@placeId", place.placeId);
                parameters.Add("@name", place.name);
                parameters.Add("@address", place.address);
                parameters.Add("@type", place.type);

                try
                {
                    db.Query<Place>("update Places set name=@name, address=@address, type=@type where placeId=@placeId", parameters);
                }
                catch
                {
                    return null;
                }

                return db.Get<Place>(new Place { placeId = place.placeId });
            }
        }

        public string reservePlace(Reserve reserve)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@customerId", reserve.customerId);
                parameters.Add("@placeId", reserve.placeId);
                parameters.Add("@price", reserve.price);
                parameters.Add("@reserveDate", reserve.reserveDate);
                parameters.Add("@dateAdded", reserve.dateAdded);

                // check if there is a reserve in this date
                var res = db.Query<Reserve>("select * from Reservation where reserveDate=@reserveDate and placeId=@placeId", parameters);

                if (res.Count() > 0) return "امکان ثبت این مکان در این زمان برای شما نمیباشد";

                // submit the reservation
                try
                {
                    db.Query<Reserve>("insert into Reservation(customerId, placeId, price, reserveDate, dateAdded) values(@customerId, @placeId, @price, @reserveDate, @dateAdded)", parameters);
                }
                catch
                {
                    return "متاسفانه مشکلی در ارتباط با سرور رخ داده است";
                }

                return "عملیات با موفقیت انجام شد";
            }
        }

        public List<Place> searchByName(string s, int page)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("s", s, DbType.String);
                parameters.Add("page", page, DbType.Int32);

                List<Place> res = db.Query<Place>("search_byName", parameters, commandType: CommandType.StoredProcedure).ToList();

                return res;
            }
        }

        public List<Place> searchByType(string s, int page)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("s", s, DbType.String);
                parameters.Add("page", page, DbType.Int32);

                List<Place> res = db.Query<Place>("search_byType", parameters, commandType: CommandType.StoredProcedure).ToList();

                return res;
            }
        }

        public List<Place> searchAll(string name, string type, int page)
        {
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("conn")))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("name", name, DbType.String);
                parameters.Add("type", type, DbType.String);
                parameters.Add("page", page, DbType.Int32);
                List<Place> res = new List<Place>();
                try
                {
                    res = db.Query<Place>("searchAll", parameters, commandType: CommandType.StoredProcedure).ToList();
                }
                catch
                {
                    return null;
                }

                return res;
            }
        }

    }
}
