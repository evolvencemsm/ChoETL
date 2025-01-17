﻿using ChoETL;
using SharpYaml;
using SharpYaml.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace ChoYamlWriterTest
{
    [ChoYamlRecordObject(ObjectValidationMode = ChoObjectValidationMode.MemberLevel, ErrorMode = ChoErrorMode.ReportAndContinue)]
    public class Emp
    {
        [DisplayName("Id")]
        public int ID { get; set; }
        public string Name { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        [DisplayName("street")]
        [StringLength(maximumLength: 5)]
        [ChoIgnoreMember]
        public string Street { get; set; }
        public string City { get; set; }
    }

    class Program
    {
        static void HelloWorld()
        {
            StringBuilder yaml = new StringBuilder();
            using (var w = new ChoYamlWriter(yaml)
                .WithField("Id", valueConverter: o => (int)o + 1)
                .WithField("Name")
                )
            {
                w.Write(new
                {
                    Id = 1,
                    Name = "Tom"
                });
            }

            Console.WriteLine(yaml.ToString());
        }

        static void SerializeDictionary()
        {
            StringBuilder yaml = new StringBuilder();
            using (var w = new ChoYamlWriter(yaml)
                )
            {
                w.Write(new Dictionary<string, object>()
                {
                    ["1"] = 1,
                    ["2"] = 2
                });
            }

            Console.WriteLine(yaml.ToString());
        }

        static void SerializeDictionaryArray()
        {
            StringBuilder yaml = new StringBuilder();
            List<Dictionary<string, object>> arr = new List<Dictionary<string, object>>();
            arr.Add(new Dictionary<string, object>()
            {
                ["1"] = 1,
                ["2"] = 2
            });
            arr.Add(new Dictionary<string, object>()
            {
                ["3"] = 1,
                ["4"] = 2
            });
            using (var w = new ChoYamlWriter(yaml)
                )
            {
                w.Write(arr);
            }

            Console.WriteLine(yaml.ToString());
        }

        static void SerializeList()
        {
            StringBuilder yaml = new StringBuilder();
            var x = new List<string>()
                {
                    "Tom",
                    "Mark"
                };
            using (var w = new ChoYamlWriter(yaml)
                )
            {
                w.Write(x);
            }

            Console.WriteLine(yaml.ToString());
        }

        static void SerializeListAsWhold()
        {
            StringBuilder yaml = new StringBuilder();
            object x = new List<string>()
                {
                    "Tom",
                    "Mark"
                };
            using (var w = new ChoYamlWriter(yaml)
                )
            {
                w.Write(x);
            }

            Console.WriteLine(yaml.ToString());
        }

        static void SerializeArrayList()
        {
            StringBuilder yaml = new StringBuilder();
            ArrayList arr = new ArrayList ();
            arr.Add(new Dictionary<string, object>()
            {
                ["1"] = 1,
                ["2"] = 2
            });
            arr.Add(new Dictionary<string, object>()
            {
                ["3"] = 1,
                ["4"] = 2
            });
            using (var w = new ChoYamlWriter(yaml)
                )
            {
                w.Write(arr);
            }

            Console.WriteLine(yaml.ToString());
        }

        static void SerializeValueTypes()
        {
            StringBuilder yaml = new StringBuilder();
            var x = new List<int>()
                {
                    1,
                    2
                };
            using (var w = new ChoYamlWriter(yaml)
                )
            {
                w.Write(x);
            }

            Console.WriteLine(yaml.ToString());
        }

        static void SerializeNullableTypes()
        {
            StringBuilder yaml = new StringBuilder();
            object x = new List<int?>()
                {
                    1,
                    2
                };
            using (var w = new ChoYamlWriter(yaml)
                )
            {
                w.Write(x);
            }

            Console.WriteLine(yaml.ToString());
        }

        static void POCOTest()
        {
            StringBuilder yaml = new StringBuilder();
            Emp e1 = new Emp
            {
                ID = 1,
                Name = "Tom",

                Address = new Address
                {
                    Street = "1 Main Street",
                    City = "NYC"
                }
            };

            using (var w = new ChoYamlWriter<Emp>(yaml)
                )
            {
                w.Write(e1);
            }

            Console.WriteLine(yaml.ToString());

        }

        static void WriteDataTableTest()
        {
            string csv = @"Id, Name
1, Tom
2, Mark";

            StringBuilder yaml = new StringBuilder();
            using (var r = ChoCSVReader.LoadText(csv)
                .WithFirstLineHeader()
                )
            {
                var dt = r.AsDataTable();
                dt.Print();

                using (var w = new ChoYamlWriter(yaml)
                    //.ReuseSerializerObject(false)
                    .UseYamlSerialization(true)
                    )
                {
                    w.Write(dt);
                }
            }

            using (var r = ChoYamlReader.LoadText(yaml.ToString())
                )
            {
                r.AsDataTable().Print();
            }

            Console.WriteLine(yaml.ToString());
        }

        static void SerializeValueTypesOneAtATime()
        {
            StringBuilder yaml = new StringBuilder();
            using (var w = new ChoYamlWriter<int>(yaml)
                )
            {
                w.Write(1);
                w.Write(2);
            }

            Console.WriteLine(yaml.ToString());
        }

        public class MethodCall
        {
            public string MethodName { get; set; }
            public List<object> Arguments { get; set; }
        }

        static void CustomSerialization()
        {
            StringBuilder yaml = new StringBuilder();
            using (var w = new ChoYamlWriter(yaml)
                )
            {
                var rec = new MethodCall
                {
                    MethodName = "someName",
                    Arguments = new List<object>
                    {
                        "arg1",
                        "arg2"
                    }
                }.ToDictionaryFromObject(o => o.MethodName, o => o.Arguments);

                w.Write(rec);
            }

            Console.WriteLine(yaml.ToString());
        }


        public class MethodCall1
        {
            public string MethodName { get; set; }
            public List<object> Arguments { get; set; }
            [ChoFallbackValue("x")]
            [ChoDefaultValue("x")]
            public object barray { get; set; }
        }

        static void IgnoreFailedMembers()
        {
            StringBuilder yaml = new StringBuilder();
            using (var w = new ChoYamlWriter<MethodCall1>(yaml)
                )
            {
                var rec = new MethodCall1
                {
                    MethodName = "someName",
                    Arguments = new List<object>
                    {
                        "arg1",
                        "arg2"
                    },
                    barray = new EntryPointNotFoundException()
                };

                w.Write(rec);
            }

            Console.WriteLine(yaml.ToString());
        }


        public static void ExternalSortTest()
        {
            string csv = @"Id, Name, City
1, Tom, NY
2, Mark, NJ
3, Lou, FL
4, Smith, PA
5, Raj, DC
";

            StringBuilder csvOut = new StringBuilder();
            using (var r = ChoCSVReader.LoadText(csv)
                       .WithFirstLineHeader()
                   )
            {
                using (var w = new ChoCSVWriter(csvOut)
                       .WithFirstLineHeader()
                       )
                {
                    w.Write(r.ExternalSort((e1, e2) => String.Compare(e1.Name, e2.Name)));
                }
            }

            Console.WriteLine(csvOut.ToString());
        }

        static void Main(string[] args)
        {
            ChoETLFrxBootstrap.TraceLevel = System.Diagnostics.TraceLevel.Error;
            WriteDataTableTest();
            //SerializeValueTypesOneAtATime();
        }
    }

    /*
    public class DataTableTypeConverter //: IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return typeof(DataTable).IsAssignableFrom(type);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var table = new DataTable();

            parser..Expect<MappingStart>();

            ReadColumns(parser, table);
            ReadRows(parser, table);

            parser.Expect<MappingEnd>();

            return table;
        }

        private static void ReadColumns(IParser parser, DataTable table)
        {
            var columns = parser.Expect<Scalar>();
            if (columns.Value != "columns")
            {
                throw new YamlException(columns.Start, columns.End,
                                        "Expected a scalar named 'columns'");
            }

            parser.Expect<MappingStart>();
            while (parser.Allow<MappingEnd>() == null)
            {
                var columnName = parser.Expect<Scalar>();
                var typeName = parser.Expect<Scalar>();

                table.Columns.Add(columnName.Value, Type.GetType(typeName.Value));
            }
        }

        private static void ReadRows(IParser parser, DataTable table)
        {
            var columns = parser.Expect<Scalar>();
            if (columns.Value != "rows")
            {
                throw new YamlException(columns.Start, columns.End,
                                        "Expected a scalar named 'rows'");
            }

            parser.Expect<SequenceStart>();
            while (parser.Allow<SequenceEnd>() == null)
            {
                var row = table.NewRow();

                var columnIndex = 0;
                parser.Expect<SequenceStart>();
                while (parser.Allow<SequenceEnd>() == null)
                {
                    var value = parser.Expect<Scalar>();
                    var columnType = table.Columns[columnIndex].DataType;
                    row[columnIndex] = TypeConverter.ChangeType(value.Value, columnType);
                    ++columnIndex;
                }

                table.Rows.Add(row);
            }
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var table = (DataTable)value;
            emitter.Emit(new MappingStart());

            EmitColumns(emitter, table);
            EmitRows(emitter, table);

            emitter.Emit(new MappingEnd());
        }

        private static void EmitColumns(IEmitter emitter, DataTable table)
        {
            emitter.Emit(new Scalar("columns"));
            emitter.Emit(new MappingStart(null, null, true, MappingStyle.Block));
            foreach (DataColumn column in table.Columns)
            {
                emitter.Emit(new Scalar(column.ColumnName));
                emitter.Emit(new Scalar(column.DataType.AssemblyQualifiedName));
            }
            emitter.Emit(new MappingEnd());
        }

        private static void EmitRows(IEmitter emitter, DataTable table)
        {
            emitter.Emit(new Scalar("rows"));
            emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Block));

            foreach (DataRow row in table.Rows)
            {
                emitter.Emit(new SequenceStart(null, null, true, SequenceStyle.Flow));
                foreach (var item in row.ItemArray)
                {
                    var value = TypeConverter.ChangeType<string>(item);
                    emitter.Emit(new Scalar(value));
                }
                emitter.Emit(new SequenceEnd());
            }

            emitter.Emit(new SequenceEnd());
        }
    }
    */
}
