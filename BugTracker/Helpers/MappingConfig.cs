using AutoMapper;
using BugTracker.Models;
using BugTracker.Models.Interfaces;
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
            cfg.AddProfile<TicketProfile>();
            cfg.AddProfile<TicketDetailsProfile>();
            cfg.AddProfile<CreateTicketProfile>();
            cfg.AddProfile<EditTicketProfile>();
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<ProjectProfile>();
        });
    }

    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<IUserItem, UserViewModel>();
            CreateMap<IUserItem, UserRoleViewModel>();
        }
    }

    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectViewModel>()
                .ForMember(dest => dest.NumberOfMembers, opt => opt.MapFrom(src => src.Members.Count()))
                .ForMember(dest => dest.NumberOfTickets, opt => opt.MapFrom(src => src.Tickets.Count()));
        }
    }

    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket, TicketViewModel>()
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.Updated.HasValue ? src.Updated : src.Created))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.DisplayName))
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Assignee.DisplayName))
                .ForMember(dest => dest.NumberOfRevisions, opt => opt.MapFrom(src => src.Revisions.Count()))
                .ForMember(dest => dest.NumberOfAttachments, opt => opt.MapFrom(src => src.Attachments.Count()))
                .ForMember(dest => dest.NumberOfComments, opt => opt.MapFrom(src => src.Comments.Count()));
        }
    }

    public class TicketDetailsProfile : Profile
    {
        public TicketDetailsProfile()
        {
            CreateMap<Ticket, TicketDetailsViewModel>()
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.Updated.HasValue ? src.Updated : src.Created))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.DisplayName))
                .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Assignee.DisplayName))
                .ForMember(dest => dest.NumberOfRevisions, opt => opt.MapFrom(src => src.Revisions.Count()))
                .ForMember(dest => dest.NumberOfAttachments, opt => opt.MapFrom(src => src.Attachments.Count()))
                .ForMember(dest => dest.NumberOfComments, opt => opt.MapFrom(src => src.Comments.Count()))
                // TODO: Replace this line when attachment is done
                .ForMember(dest => dest.Attachments, opt => new HashSet<AttachmentViewModel>())
                // TODO: Replace this line when comment is done
                .ForMember(dest => dest.Comments, opt => new HashSet<CommentViewModel>());
            //.ForMember(dest => dest.Revisions, opt => opt.MapFrom(src => src.Revisions));
            CreateMap<TicketRevision, TicketRevisionViewModel>();
            CreateMap<TicketRevisionDetail, TicketRevisionDetailViewModel>();
        }
    }

    public class CreateTicketProfile : Profile
    {
        public CreateTicketProfile()
        {
            CreateMap<CreateTicketViewModel, Ticket>();
        }
    }

    public class EditTicketProfile : Profile
    {
        public EditTicketProfile()
        {
            CreateMap<EditTicketViewModel, Ticket>();
            CreateMap<Ticket, EditTicketViewModel>();
        }
    }
}