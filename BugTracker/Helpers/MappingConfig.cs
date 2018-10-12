using AutoMapper;
using BugTracker.Models;
using BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class MappingConfig
    {
        public static MapperConfiguration Config = new MapperConfiguration(cfg => {
            cfg.AddProfile<TicketDetailsProfile>();
        });
    }

    public class TicketDetailsProfile : Profile
    {
        public TicketDetailsProfile()
        {
            CreateMap<Ticket, TicketDetailsViewModel>()
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.DisplayName))
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Assignee.DisplayName))
                .ForMember(dest => dest.NumberOfRevisions, opt => opt.MapFrom(src => src.Revisions.Count()))
                .ForMember(dest => dest.NumberOfAttachments, opt => opt.MapFrom(src => src.Attachments.Count()))
                .ForMember(dest => dest.NumberOfComments, opt => opt.MapFrom(src => src.Comments.Count()))
                .ForMember(dest => dest.Attachments, opt => new HashSet<AttachmentViewModel>())
                .ForMember(dest => dest.Comments, opt => new HashSet<CommentViewModel>());
        }
    }
}