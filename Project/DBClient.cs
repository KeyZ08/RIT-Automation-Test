using GMap.NET;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace TestMap
{
    public static class DBClient
    {
        private static SqlConnection connection;

        static DBClient() 
        {
            if (connection == null)
            {
                connection = new SqlConnection(Properties.Settings.Default.GMapDBConnectionString);
            }
        }

        public static Marker[] GetAllMarkers()
        {
            using (SqlCommand sqlCommand = new SqlCommand("getAllMarkers", connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                try
                {
                    var result = new List<Marker>();
                    connection.Open();
                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        var marker = new Marker()
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Position = new PointLatLng(
                                reader.GetDouble(2),
                                reader.GetDouble(3)
                                )
                        };
                        result.Add(marker);
                    }
                    return result.ToArray();
                }
                catch
                {
                    MessageBox.Show("Ошибка. Не удалось извлечь данные.");
                    throw new Exception();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static int MarkerAdd(Marker marker)
        {
            using (SqlCommand sqlCommand = new SqlCommand("newMarker", connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@MarkerName", marker.Name);
                sqlCommand.Parameters.AddWithValue("@Latitude", marker.Position.Lat);
                sqlCommand.Parameters.AddWithValue("@Longitude", marker.Position.Lng);
                sqlCommand.Parameters.Add(new SqlParameter("@MarkerID", SqlDbType.Int));
                sqlCommand.Parameters["@MarkerID"].Direction = ParameterDirection.Output; try
                {
                    connection.Open();
                    sqlCommand.ExecuteNonQuery();
                    var newId = (int)sqlCommand.Parameters["@MarkerID"].Value;
                    return newId;
                }
                catch
                {
                    MessageBox.Show("ID не возвращен, не удалось создать новый Marker.");
                    throw new Exception();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static void MarkerDelete(Marker marker)
        {
            using (SqlCommand sqlCommand = new SqlCommand("deleteMarker", connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@MarkerID", marker.Id);

                StandartExecuteNonQuery(sqlCommand, "Не удалось удалить Marker.");
            }
        }

        public static void MarkerUpdate(Marker marker)
        {
            using (SqlCommand sqlCommand = new SqlCommand("updateMarker", connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;

                sqlCommand.Parameters.AddWithValue("@MarkerName", marker.Name);
                sqlCommand.Parameters.AddWithValue("@Latitude", marker.Position.Lat);
                sqlCommand.Parameters.AddWithValue("@Longitude", marker.Position.Lng);
                sqlCommand.Parameters.AddWithValue("@MarkerID", marker.Id);

                StandartExecuteNonQuery(sqlCommand, "Не удалось обновить данные.");
            }
        }

        private static void StandartExecuteNonQuery(SqlCommand sqlCommand, string exceptionText)
        {
            try
            {
                connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch
            {
                MessageBox.Show(exceptionText);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
