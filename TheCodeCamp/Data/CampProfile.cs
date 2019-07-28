using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using TheCodeCamp.Models;

namespace TheCodeCamp.Data
{
    //Constructs the auto mapper profile
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            //Create a map from camp to camp model
            CreateMap<Camp, CampModel>()
                /*
                 * For member Venue(not LocationVenue) in CampModel, we want the value to be
                 * mapped to the venue name s
                 */
                .ForMember(m => m.Venue, opt => opt.MapFrom(d => d.Location.VenueName));
        }
    }
}