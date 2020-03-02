﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceCore.Models
{
    public class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();
           
            if(context.Products.Any())
            {
                return;
            }

            var products = new Product[]
            {
                new Product{ProductName="Echo Dot",
                    Description="Meet Echo Dot - Our most popular smart speaker with a fabric design. It is our most compact smart speaker that fits perfectly into small spaces.",
                    ImagePath="/assets/images/2020/02/07/echo-dot.jpg",
                    Pricing=44.99,
                    ShippingCost=0
                },
                new Product{ProductName="AirPods Wireless Bluetooth Headset",
                    Description="Just take them out and they're ready to use with all your devices, put them in your ears and they connect instantly",
                    ImagePath="/assets/images/2020/02/07/air-pod.jpg",
                    Pricing=244.68,
                    ShippingCost=0
                },
                new Product{ProductName="Lenovo ThinkPad X1 Carbon 7th Gen Laptop",
                    Description="Processor: 8th Gen Intel Core i5-8265U (1.60GHz, up to 3.90GHz with Turbo Boost, 4 Cores, 6MB Cache).\nMemory: 8GB LPDDR3 2133MHz (Soldered). Graphics: Integrated Intel UHD 620 Graphics",
                    ImagePath="/assets/images/2020/02/07/air-pod.jpg",
                    Pricing=1386.57,
                    ShippingCost=136.68
                },
                new Product{ProductName="Nintendo Switch with Neon Blue and Neon Red Joy‑Con ",
                    Description="The Nintendo Switch combines the mobility of a handheld with the power of a home gaming system, offering unprecedented gaming and entertainment experiences ",
                    ImagePath="/assets/images/2020/02/07/nitendo-switch.jpg",
                    Pricing=386,
                    ShippingCost=10
                },
                new Product{ProductName="The Fountainhead",
                    Description="When The Fountainhead was first published, Ayn Rand's daringly original literary vision and her groundbreaking philosophy, Objectivism, won immediate worldwide interest and acclaim. This instant classic is the story of an intransigent young architect, his violent battle against conventional standards, and his explosive love affair with a beautiful woman who struggles to defeat him. This edition contains a special afterword by Rand’s literary executor, Leonard Peikoff, which includes excerpts from Ayn Rand’s own notes on the making of The Fountainhead. As fresh today as it was then, here is a novel about a hero—and about those who try to destroy him.",
                    ImagePath="/assets/images/2020/02/07/the-foundtainhead.jpg",
                    Pricing=24.72,
                    ShippingCost=0
                },
                new Product{ProductName="Kindle Paperwhite – Now Waterproof with 2x the Storage",
                    Description="The thinnest, lightest Kindle Paperwhite yet—with a flush-front design and 300 ppi glare-free display that reads like real paper even in bright sunlight. ",
                    ImagePath="/assets/images/2020/02/07/kindle.jpg",
                    Pricing=139.99,
                    ShippingCost=0
                }
            };
            foreach(Product product in products)
            {
                context.Products.Add(product);
            }
            context.SaveChanges();
        }
    }
}
