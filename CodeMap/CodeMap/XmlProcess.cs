using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeMap
{
    class XmlProcess
    {
        public static void SaveCFileInfo2XML(CFileInfo cfi)
        {
            XDocument xdoc = new XDocument();
            // 创建文件节点
            XElement xelmt = new XElement("source_file_name", CSourceProcess.GetFileName(cfi.full_name));
            XElement subElmt = null;

            // inlude头文件列表
            if (0 != cfi.include_file_list.Count)
            {
                subElmt = new XElement("include_file_list");
                foreach (string incFile in cfi.include_file_list)
                {
                    XElement nodeElmt = new XElement("include_file", incFile);
                    subElmt.Add(nodeElmt);
                }
                xelmt.Add(subElmt);
            }

            // 宏定义列表
            if (0 != cfi.macro_define_list.Count)
            {
                subElmt = new XElement("macro_define_list");
                foreach (MacroDefineInfo mdi in cfi.macro_define_list)
                {
                    XElement node1 = new XElement("name", mdi.name);
                    XElement node2 = null;
                    if (0 != mdi.paras.Count)
                    {
                        string paraStr = "";
                        int idx = 0;
                        foreach (string para in mdi.paras)
                        {
                            paraStr += para;
                            if (idx != mdi.paras.Count - 1)
                            {
                                paraStr += ", ";
                            }
                            idx++;
                        }
                        node2 = new XElement("parameters", paraStr.Trim());
                        node1.Add(node2);
                    }

                    node2 = new XElement("value", mdi.value);
                    node1.Add(node2);

                    subElmt.Add(node1);
                }
                xelmt.Add(subElmt);
            }

            // 用户定义类型列表
            if (0 != cfi.user_def_type_list.Count)
            {
                subElmt = new XElement("user_type_define_list");
                foreach (UsrDefineTypeInfo udi in cfi.user_def_type_list)
                {
                    XElement node1 = new XElement("type", udi.type);
                    string nameStr = "";
                    int idx = 0;
                    foreach (string name in udi.nameList)
                    {
                        nameStr += name;
                        if (idx != udi.nameList.Count - 1)
                        {
                            nameStr += ", ";
                        }
                        idx++;
                    }
                    XElement node2 = new XElement("names", nameStr.Trim());
                    node1.Add(node2);

                    node2 = new XElement("members");
                    foreach (string member in udi.memberList)
                    {
                        XElement node3 = new XElement("member", member);
                        node2.Add(node3);
                    }
                    node1.Add(node2);

                    node2 = new XElement("body_start", "row: " + udi.body_start_pos.row_num.ToString() + "; col: " + udi.body_start_pos.col_num.ToString());
                    node1.Add(node2);
                    node2 = new XElement("body_end", "row: " + udi.body_end_pos.row_num.ToString() + "; col: " + udi.body_end_pos.col_num.ToString());
                    node1.Add(node2);

                    subElmt.Add(node1);
                }
                xelmt.Add(subElmt);
            }

            // typedef列表
            if (0 != cfi.type_define_list.Count)
            {
                subElmt = new XElement("typedef_list");
                foreach (TypeDefineInfo tdi in cfi.type_define_list)
                {
                    XElement node1 = new XElement("new_type", tdi.new_type_name);
                    XElement node2 = new XElement("original_type", tdi.old_type_name);
                    node1.Add(node2);
                    subElmt.Add(node1);
                }
                xelmt.Add(subElmt);
            }

            // 全局量声明列表
            if (0 != cfi.global_var_declare_list.Count)
            {
                subElmt = new XElement("global_declare_list");
                foreach (GlobalVarInfo gvi in cfi.global_var_declare_list)
                {
                    XElement node1 = new XElement("name", gvi.name);
                    XElement node2 = new XElement("type", gvi.type);
                    node1.Add(node2);

                    if (0 != gvi.qualifiers.Count)
                    {
                        node2 = new XElement("qualifiers");
                        foreach (string qlf in gvi.qualifiers)
                        {
                            XElement node3 = new XElement("qualifier", qlf);
                            node2.Add(node3);
                        }
                        node1.Add(node2);
                    }

                    if ("" != gvi.array_size_string)
                    {
                        node2 = new XElement("array_size_string", gvi.array_size_string);
                        node1.Add(node2);
                    }
                    if ("" != gvi.initial_string)
                    {
                        node2 = new XElement("initial_string", gvi.initial_string);
                        node1.Add(node2);
                    }
                    subElmt.Add(node1);
                }
                xelmt.Add(subElmt);
            }

            // 全局量定义列表
            if (0 != cfi.global_var_define_list.Count)
            {
                subElmt = new XElement("global_define_list");
                foreach (GlobalVarInfo gvi in cfi.global_var_define_list)
                {
                    XElement node1 = new XElement("name", gvi.name);
                    XElement node2 = new XElement("type", gvi.type);
                    node1.Add(node2);

                    if (0 != gvi.qualifiers.Count)
                    {
                        node2 = new XElement("qualifiers");
                        foreach (string qlf in gvi.qualifiers)
                        {
                            XElement node3 = new XElement("qualifier", qlf);
                            node2.Add(node3);
                        }
                        node1.Add(node2);
                    }

                    if ("" != gvi.array_size_string)
                    {
                        node2 = new XElement("array_size_string", gvi.array_size_string);
                        node1.Add(node2);
                    }
                    if ("" != gvi.initial_string)
                    {
                        node2 = new XElement("initial_string", gvi.initial_string);
                        node1.Add(node2);
                    }
                    subElmt.Add(node1);
                }
                xelmt.Add(subElmt);
            }

            // 函数声明列表
            if (0 != cfi.fun_declare_list.Count)
            {
                subElmt = new XElement("function_declare_list");
                foreach (CFunctionInfo fi in cfi.fun_declare_list)
                {
                    XElement node1 = new XElement("func_name", fi.name);
                    XElement node2 = null;
                    if (0 != fi.qualifiers.Count)
                    {
                        node2 = new XElement("qualifiers");
                        foreach (string qlf in fi.qualifiers)
                        {
                            XElement node3 = new XElement("qualifier", qlf);
                            node2.Add(node3);
                        }
                        node1.Add(node2);
                    }

                    if (0 != fi.paras.Count)
                    {
                        string paraStr = "";
                        int idx = 0;
                        foreach (string para in fi.paras)
                        {
                            paraStr += para;
                            if (idx != fi.paras.Count - 1)
                            {
                                paraStr += ", ";
                            }
                            idx++;
                        }
                        node2 = new XElement("parameters", paraStr.Trim());
                        node1.Add(node2);
                    }
                    subElmt.Add(node1);
                }
                xelmt.Add(subElmt);
            }

            // 函数定义列表
            if (0 != cfi.fun_define_list.Count)
            {
                subElmt = new XElement("function_define_list");
                foreach (CFunctionInfo fi in cfi.fun_define_list)
                {
                    XElement node1 = new XElement("func_name", fi.name);
                    XElement node2 = null;
                    if (0 != fi.qualifiers.Count)
                    {
                        node2 = new XElement("qualifiers");
                        foreach (string qlf in fi.qualifiers)
                        {
                            XElement node3 = new XElement("qualifier", qlf);
                            node2.Add(node3);
                        }
                        node1.Add(node2);
                    }

                    if (0 != fi.paras.Count)
                    {
                        string paraStr = "";
                        int idx = 0;
                        foreach (string para in fi.paras)
                        {
                            paraStr += para;
                            if (idx != fi.paras.Count - 1)
                            {
                                paraStr += ", ";
                            }
                            idx++;
                        }
                        node2 = new XElement("parameters", paraStr.Trim());
                        node1.Add(node2);
                    }

                    node2 = new XElement("body_start", "row: " + fi.body_start_pos.row_num.ToString() + "; col: " + fi.body_start_pos.col_num.ToString());
                    node1.Add(node2);
                    node2 = new XElement("body_end", "row: " + fi.body_end_pos.row_num.ToString() + "; col: " + fi.body_end_pos.col_num.ToString());
                    node1.Add(node2);

                    subElmt.Add(node1);
                }
                xelmt.Add(subElmt);
            }

            xdoc.Add(xelmt);
            xdoc.Save(cfi.full_name + ".xml");
        }
    }
}
