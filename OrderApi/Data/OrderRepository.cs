﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace OrderApi.Data;

public class OrderRepository : IRepository<Order>
{
    private readonly OrderApiContext db;

    public OrderRepository(OrderApiContext context)
    {
        db = context;
    }

    Order IRepository<Order>.Add(Order entity)
    {
        if (entity.Date == null)
            entity.Date = DateTime.Now;

        var newOrder = db.Add(entity).Entity;
        db.SaveChanges();
        return newOrder;
    }


    void IRepository<Order>.Edit(Order entity)
    {
        db.Entry(entity).State = EntityState.Modified;
        db.SaveChanges();
    }

    Order IRepository<Order>.Get(int id)
    {
        return db.Orders.Include(o => o.OrderItems).FirstOrDefault(o => o.Id == id);
    }

    IEnumerable<Order> IRepository<Order>.GetAll()
    {
        return db.Orders.Include(o => o.OrderItems).ToList();
    }

    void IRepository<Order>.Remove(int id)
    {
        var order = db.Orders.FirstOrDefault(p => p.Id == id);
        db.Orders.Remove(order);
        db.SaveChanges();
    }
}