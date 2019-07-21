using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System;
using FluentValidation;
using APITestRegister.Presentation.WebAPI.Models;

namespace APITestRegister.Presentation.WebAPI.Controllers
{
    public class BaseController : Controller
    {
        public BaseController()
        {
        }

        internal IActionResult ExecMethod<T>(Action validation, Func<T> method)
        {
            try
            {
                validation?.Invoke();
                var @return = method();
                return Ok(@return);
            }
            catch (Exception ex)
            {
                var @return = CheckException(ex);
                if (@return != null)
                    return BadRequest(@return);
                throw ex;
            }
        }

        internal IActionResult ExecMethod<T, T2>(Action validation, Func<T> method)
        {
            try
            {
                validation?.Invoke();
                var @return = method();
                return Ok(Mapper.Map<T2>(@return));
            }
            catch (Exception ex)
            {
                var @return = CheckException(ex);
                if (@return != null)
                    return BadRequest(@return);
                throw ex;
            }
        }

        internal void ValidateEntry<T>(AbstractValidator<T> validator, T vm)
        {
            var val = validator.Validate(vm);
            if (!val.IsValid)
            {
                throw new BusinessException(val.Errors);
            }
        }

        internal dynamic CheckException(Exception ex)
        {
            if (ex.GetType() == typeof(BusinessException)
                ||
                (ex.InnerException != null && ex.InnerException.GetType() == typeof(BusinessException)))
            {
                BusinessException bex = (BusinessException)ex;
                return bex.ErrorList;
            }

            return null;
        }
    }
}