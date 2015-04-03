using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;

namespace ExcelRead
{
    struct GoodsStruct
    {
        public int goods_id;
        public string goods_name;
        public decimal goods_price_jpy;
        public float goods_weight;
        public decimal goods_price_weight_jpy;
        public decimal goods_price_cost_cny;
        public decimal goods_price_agent;
        public decimal goods_price_sell;
        public float goods_gain_rate;
    }

    struct ExportStruct
    {
        public string agent_name;
        public int goods_id;
        public string goods_name;
        public int goods_number;
        public decimal total_cost;
    }

    struct CloseStruct
    {
        public int close_id;
        public string agent_name;
        public int goods_id;
        public string goods_name;
        public int goods_number;
        public decimal total_cost;
        public decimal close_price_cny;
        public decimal present_price_cny;
        public decimal pure_gain_cny;
        public decimal money_from_agent_cny;
    }
}

