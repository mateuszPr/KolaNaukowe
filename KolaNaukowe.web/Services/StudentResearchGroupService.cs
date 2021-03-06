﻿using AutoMapper;
using KolaNaukowe.web.Dtos;
using KolaNaukowe.web.Extensions;
using KolaNaukowe.web.Models;
using KolaNaukowe.web.Repositories;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using KolaNaukowe.web.Dtos.Api;
using KolaNaukowe.web.Data;

namespace KolaNaukowe.web.Services
{
    public class StudentResearchGroupService : IStudentResearchGroupService
    {

        private readonly IGenericRepository<StudentResearchGroup> _genericRepository;
        private readonly IMapper _mapper;
        private readonly ISubjectService _subjectService;
        private KolaNaukoweDbContext _context;



        public StudentResearchGroupService(KolaNaukoweDbContext context, IGenericRepository<StudentResearchGroup> genericRepository, IMapper mapper, ISubjectService subjectService)
        {
            _genericRepository = genericRepository;
            _mapper = mapper;
            _subjectService = subjectService;
            _context = context;
        }

        public StudentResearchGroupDto Add(StudentResearchGroup newStudentResearchGroup)
        {
            var studentResearchGroup = _genericRepository.Add(newStudentResearchGroup);
            return _mapper.Map<StudentResearchGroup, StudentResearchGroupDto>(studentResearchGroup);
        }

       
        public StudentResearchGroupDto Get(int id)
        {
            var studentResearchGroup = _genericRepository.Get(g => g.Id.Equals(id), g => g.Subjects);
            return _mapper.Map<StudentResearchGroup, StudentResearchGroupDto>(studentResearchGroup);
        }

        public IEnumerable<StudentResearchGroupDto> GetAll()
        {
            var studentResearchGroups = _genericRepository.GetAll(g => g.Subjects)
                                                          .OrderBy(c => c.Name);
                                                                                                                                     
            return _mapper.Map<IEnumerable<StudentResearchGroup>, IEnumerable<StudentResearchGroupDto>>(studentResearchGroups);                      
        }

        public IEnumerable<string> GetAllSubjects()
        {
            var subjects = from l in _genericRepository.GetAll()
                           from s in l.Subjects
                           select s.Name;
            return subjects.ToList();
        }


        public IEnumerable<StudentResearchGroupDto> FilterByName(IEnumerable<StudentResearchGroupDto> source, string searchString)
        {
            var filteringResult = source.Where(s => s.Name.Contains(searchString, System.StringComparison.CurrentCultureIgnoreCase));
            return filteringResult;
        }

        public IEnumerable<StudentResearchGroupDto> FilterBySubject(IEnumerable<StudentResearchGroupDto> source, string searchString)
        {
            var filteringResult = source.Where(s => s.Subjects.Any(u => u.Name == searchString));
            return filteringResult;
        }

        public void Remove(int id)
        {
            var studentResearchGroup = Get(id);

            if (studentResearchGroup != null)
            {
                var subjects = _context.Subjects.Where(s => s.researchGroups.Id == studentResearchGroup.Id).ToList();

                foreach (var subject in subjects)
                {
                    _subjectService.Remove(subject.Id);
                }
            }
          
            _genericRepository.Remove(id);
        }

        public bool Update(int id, AddEditStudentResearchGroupDto studentResearchGroup)
        {
            var group = Get(id);
            if (group != null)
            {
                group.Name = studentResearchGroup.Name;
                group.Leader = studentResearchGroup.Leader;
                group.Attendant = studentResearchGroup.Attendant;
                group.Department = studentResearchGroup.Department;
                group.CreatedAt = studentResearchGroup.CreatedAt;
                group.Description = studentResearchGroup.Description;

                var updatedStudentResearchGroup = _mapper.Map<StudentResearchGroupDto, StudentResearchGroup>(group);

                return _genericRepository.Update(updatedStudentResearchGroup);
            }      
            return false;
        }

        public void Update(StudentResearchGroup item)
        {
            _genericRepository.Update(item);
        }

        public IEnumerable<WriteStudentResearchGroupDto> WriteAll()
        {
            var studentResearchGroups = _genericRepository.GetAll(g => g.Subjects)
                                                          .OrderBy(c => c.Name);
            return _mapper.Map<IEnumerable<StudentResearchGroup>, IEnumerable<WriteStudentResearchGroupDto>>(studentResearchGroups);
        }

        public WriteDetailsStudentResearchGroupDto WriteDetails(int id)
        {
            var studentResearchGroup = _genericRepository.Get(g => g.Id.Equals(id), g => g.Subjects);
            return _mapper.Map<StudentResearchGroup, WriteDetailsStudentResearchGroupDto>(studentResearchGroup);
        }

        public void Add(AddEditStudentResearchGroupDto studentResearchGroup)
        {
            var newStudentResearchGroup = _mapper.Map<AddEditStudentResearchGroupDto, StudentResearchGroup>(studentResearchGroup);
            _genericRepository.Add(newStudentResearchGroup);
        }
    }
}
