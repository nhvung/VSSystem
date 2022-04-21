using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Text;

namespace VSSystem.ThirdParty.Google.Maps.Geocode
{
    public class GMapGeocodeProcess
    {
        public static GMapInfoResult GetGeocodeMapInfo(string address, string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(address)) return null;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var en = Uri.EscapeUriString(address);
                byte[] bData = new WebClient().DownloadData(new Uri(string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}", en, key == "" ? "AIzaSyDarm8iEY4j3hjw2GkGGMZlfkZKtl7mU5M" : key)));
                string resData = Encoding.UTF8.GetString(bData);
                GMapInfoResult gREs = JsonConvert.DeserializeObject<GMapInfoResult>(resData);
                return gREs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static GMapInfoResult GetGeocodeMapInfo(string address, string stateName, string key = "")
        {
            try
            {
                //https://maps.googleapis.com/maps/api/geocode/json?address={0}&components=administrative_area:{2}&key={1}
                if (string.IsNullOrEmpty(address)) return null;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var en = Uri.EscapeUriString(address);
                var stateNameEncode = Uri.EscapeUriString(stateName);
                byte[] bData = new WebClient().DownloadData(new Uri(string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}&components=administrative_area:{2}", en, key == "" ? "AIzaSyDarm8iEY4j3hjw2GkGGMZlfkZKtl7mU5M" : key, stateNameEncode)));
                string resData = Encoding.UTF8.GetString(bData);
                GMapInfoResult gREs = JsonConvert.DeserializeObject<GMapInfoResult>(resData);
                return gREs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static GMapGeocode GetGeocodeInfo(string address, string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(address)) return null;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var en = Uri.EscapeUriString(address);
                byte[] bData = new WebClient().DownloadData(new Uri(string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}", en, key == "" ? "AIzaSyDarm8iEY4j3hjw2GkGGMZlfkZKtl7mU5M" : key)));
                string resData = Encoding.UTF8.GetString(bData);
                GMapInfoResult gRes = JsonConvert.DeserializeObject<GMapInfoResult>(resData);
                return gRes.Results?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static GMapGeocode GetGeocodeInfo(string address, string stateName, string key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(address)) return null;
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var en = Uri.EscapeUriString(address);
                var stateNameEncode = Uri.EscapeUriString(stateName);
                byte[] bData = new WebClient().DownloadData(new Uri(string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}&components=administrative_area:{2}", en, key == "" ? "AIzaSyDarm8iEY4j3hjw2GkGGMZlfkZKtl7mU5M" : key, stateNameEncode)));
                string resData = Encoding.UTF8.GetString(bData);
                GMapInfoResult gRes = JsonConvert.DeserializeObject<GMapInfoResult>(resData);
                return gRes.Results?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
