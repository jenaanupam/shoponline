﻿using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    /// <summary>
    /// "There's some repetition here - couldn't we have some the sync methods call the async?"
    /// https://blogs.msdn.microsoft.com/pfxteam/2012/04/13/should-i-expose-synchronous-wrappers-for-asynchronous-methods/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EfRepository<T> : IRepository<T>, IAsyncRepository<T> where T : BaseEntity
    {
        protected readonly CatalogContext _dbContext;

        public EfRepository(CatalogContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public T GetSingleBySpec(ISpecification<T> spec)
        {
            return List(spec).FirstOrDefault();
        }


        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> ListAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public async Task<List<T>> ListAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public IEnumerable<T> List(ISpecification<T> spec)
        {
            //if (typeof(T) == typeof(CatalogItem))
            //{

            //    List<CatalogItem> catalogresult = new List<CatalogItem>();

            //    string connstr = "server=10.0.0.77;port=3306;database=cf_f4acc8ac_b5e6_4d77_aeb5_5487d4a592bc;user=blMhl1vXt7ULn05w;password=IaOADel876MdImsJ;";
            //    connstr = "server=localhost;port=3306;database=testentityfwseeded;user=root;password=anupam;";
            //    string command = "SELECT * FROM catalogitem;";
                 
            //    using (MySqlConnection conn = new MySqlConnection(connstr))
            //    {
            //        conn.Open();
            //        string select = command;
            //        MySqlCommand cmd = new MySqlCommand(command, conn);
            //        using (MySqlDataReader dr = cmd.ExecuteReader())
            //        {
            //            int i = 1;
            //            while (dr.Read())
            //            {
            //                catalogresult.Add(new CatalogItem
            //                {
            //                    Name = dr.GetString("Name"),
            //                    CatalogBrandId =  dr.GetInt32("CatalogBrandId"),
            //                    CatalogTypeId = dr.GetInt32("CatalogTypeId"),
            //                     PictureUri = dr.GetString("PictureUri"),
            //                    Description = dr.GetString("Description"),
            //                    Price = dr.GetInt32("Price"),
            //                    Id= dr.GetInt32("UniqueId"),
                                
            //                });
            //                i++;
            //            }
            //        }

            //    }
            //    return (IEnumerable < T>)Convert.ChangeType(catalogresult, typeof(IEnumerable< T>));
                 

            //}
            //else
            //{
                // fetch a Queryable that includes all expression-based includes
                var queryableResultWithIncludes = spec.Includes
                    .Aggregate(_dbContext.Set<T>().AsQueryable(),
                        (current, include) => current.Include(include));

                // modify the IQueryable to include any string-based include statements
                var secondaryResult = spec.IncludeStrings
                    .Aggregate(queryableResultWithIncludes,
                        (current, include) => current.Include(include));

                // return the result of the query using the specification's criteria expression
                return secondaryResult
                                .Where(spec.Criteria)
                                .AsEnumerable();
          //  }
           
        }
        public async Task<List<T>> ListAsync(ISpecification<T> spec)
        {
            // fetch a Queryable that includes all expression-based includes
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(_dbContext.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            // return the result of the query using the specification's criteria expression
            return await secondaryResult
                            .Where(spec.Criteria)
                            .ToListAsync();
        }

        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }
        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
