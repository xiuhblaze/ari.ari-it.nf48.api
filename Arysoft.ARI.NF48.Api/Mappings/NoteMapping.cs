using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using System.Collections.Generic;

namespace Arysoft.ARI.NF48.Api.Mappings
{
    public class NoteMapping
    {
        public static IEnumerable<NoteItemDto> NotesToListDto(IEnumerable<Note> items)
        {
            var itemsDto = new List<NoteItemDto>();

            foreach (var item in items)
            {
                itemsDto.Add(NoteToItemDto(item));
            }

            return itemsDto;
        } // NotesToListDto

        public static NoteItemDto NoteToItemDto(Note item)
        {
            return new NoteItemDto
            {
                ID = item.ID,
                OwnerID = item.OwnerID,
                Text = item.Text,
                Status = item.Status,
                Created = item.Created,
                Updated = item.Updated,
                UpdatedUser = item.UpdatedUser
            };
        } // NoteToItemDto

        public static Note ItemAddDtoToNote(NotePostDto itemDto)
        {
            return new Note
            {
                OwnerID = itemDto.OwnerID,
                Text = itemDto.Text,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemAddDtoToNote

        public static Note ItemEditDtoToNote(NotePutDto itemDto)
        {
            return new Note
            {
                ID = itemDto.ID,
                Text = itemDto.Text,
                Status = itemDto.Status,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemEditDtoToNote

        public static Note ItemDeleteDtoToNote(NoteDeleteDto itemDto)
        {
            return new Note
            {
                ID = itemDto.ID,
                UpdatedUser = itemDto.UpdatedUser
            };
        } // ItemDeleteDtoToNote
    }
}