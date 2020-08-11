using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;


namespace PGTAWPF
{

    /// <summary>
    /// Airport class. It allows defining each airport as an object with its parameters
    /// </summary>
    /// <param name="AirportCoords">Coordenates of the airport</param>
    /// <param name="AirportName">Name of the airport </param>
    /// <param name="CityName"> Name of the city where the airport is </param>
    /// <param name="CCAAName">Name of the autonomous community (region of spain) where the airport is</param>
    class Airport
    {
        public PointLatLng AirportCoords;
        public string AirportName;
        public string CityName;
        public string CCAAName;

        public Airport (PointLatLng Airportcoords, string Airportname, string Cityname, string CCAAname)
        {
            this.AirportCoords = Airportcoords;
            this.AirportName = Airportname;
            this.CityName = Cityname;
            this.CCAAName = CCAAname;
        }
    }

    #region Airports List 

    /// <summary>
    /// List with the most important airports in Spain.
    /// </summary>
    class AirportsList
    {
        public List<Airport> Airportslist = new List<Airport>();

        public AirportsList()
        {
            Airport Barcelona = new Airport(new PointLatLng(41.295795, 2.083194), "LEBL", "Barcelona", "CAT"); //BARCELONA
            Airport Asturias = new Airport(new PointLatLng(43.563796, -6.035035), "LEAS", "Oviedo", "AST"); //ASTURIAS
            Airport Palma = new Airport(new PointLatLng(39.553419, 2.737091), "LEPA", "Palma", "I.B."); //PALMA
            Airport Santiago = new Airport(new PointLatLng(42.898331, -8.415872), "LEST", "Santiago", "GLC"); //SANTIAGO
            Airport Barajas = new Airport(new PointLatLng(40.488124, -3.563954), "LEMD", "Barajas", "MAD"); //BARAJAS
            Airport Tenerife = new Airport(new PointLatLng(28.482777, -16.342842), "GCXO", "La Laguna", "TNF"); //TENERIFE
            Airport Malaga = new Airport(new PointLatLng(36.675179, -4.496361), "LEMG", "Málaga", "MÁL"); //MALAGA
            Airport Bilbao = new Airport(new PointLatLng(43.301323, -2.911674), "LEBB", "Bilbao", "BIL"); //BARCELONA
            Airport Alicante = new Airport(new PointLatLng(38.283475, -0.561461), "LEAL", "Alicante", "ALC"); //ASTURIAS
            Airport Granada = new Airport(new PointLatLng(37.188208, -3.777260), "LEGR", "Granada", "GRD"); //PALMA
            Airport Lanzarote = new Airport(new PointLatLng(28.947279, -13.605156), "GCRR", "Arrecife", "LAN"); //SANTIAGO
            Airport Turrillas = new Airport(new PointLatLng(37.026731, -2.263399), "", "Turrillas", "ALM"); //BARAJAS
            Airport Menorca = new Airport(new PointLatLng(39.862859, 4.219784), "LEMH", "Menorca", "I.B"); //TENERIFE
            Airport Ibiza = new Airport(new PointLatLng(38.874032, 1.371384), "LEIB", "Ibiza", "I.B"); //MALAGA
            Airport Valdespina = new Airport(new PointLatLng(42.131984, -4.425488), "Valdespina", "Palencia", "CL"); ; //BARCELONA
            Airport Paracuellos = new Airport(new PointLatLng(40.488124, -3.563954), "LEMD", "Barajas", "MAD"); //ASTURIAS
            Airport Randa = new Airport(new PointLatLng(39.525965, 2.915444), "Randa", "Mallorca", "I.B."); //PALMA
            Airport Gerona = new Airport(new PointLatLng(41.901965, 2.762977), "LEGE", "Girona", "CAT"); //BARAJAS
            Airport Espiñeiras = new Airport(new PointLatLng(43.273651, -7.975016), "Espiñeiras", "A Coruña", "GLC"); //TENERIFE
            Airport Vejer = new Airport(new PointLatLng(36.252541, -5.967858), "Vejer", "Cadiz", "CAD"); //MALAGA
            Airport Yeste = new Airport(new PointLatLng(38.366677, -2.320546), "Yeste", "Albacete", "C.L.M."); //BARCELONA
            Airport Vigo = new Airport(new PointLatLng(42.225063, -8.628839), "LEVX", "Vigo", "GLC"); //ASTURIAS
            Airport Valencia = new Airport(new PointLatLng(39.488163, -0.479649), "LEVC", "Valencia", "VLC"); //PALMA
            Airport Sevilla = new Airport(new PointLatLng(37.419241, -5.893393), "LEZL", "Sevilla", "SEV"); //SANTIAGO
            Airportslist = new List<Airport>() { Barcelona, Asturias, Palma, Santiago, Barajas, Tenerife, Malaga, Bilbao, Alicante, Granada, Lanzarote, Turrillas, Menorca, Ibiza, Valdespina, Paracuellos, Randa, Gerona, Espiñeiras, Vejer, Yeste, Vigo, Valencia, Sevilla };
        }
    }
    #endregion
}
