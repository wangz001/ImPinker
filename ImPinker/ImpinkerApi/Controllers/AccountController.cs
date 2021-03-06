﻿using System;
using System.Net.Http;
using System.Web.Http;
using ImBLL;
using ImModel;
using ImpinkerApi.Common;
using ImpinkerApi.Models;

namespace ImpinkerApi.Controllers
{
    public class AccountController : BaseApiController
    {
        readonly UserBll _userBll = new UserBll();

        /// <summary>
        /// 登录验证。登录成功，返回token
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Login([FromBody]UserLoginViewModel loginViewModel)
        {
            var username = loginViewModel.Username;
            var password = loginViewModel.Password;
            var isSuccess = false;
            var description = string.Empty;
            var returnUserVm = new UserReturnViewModel();
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                //username  用户名或者电话号码
                var users = _userBll.GetModelByUserName(username);
                if (users == null)
                {
                    description = "用户不存在";
                }
                if (users != null && users.PassWord != password)
                {
                    description = "密码错误";
                }
                if (users != null && users.PassWord == password)
                {
                    isSuccess = true;
                    description = "登录成功";
                    var tokenStr = TokenHelper.AddOrUpdateToken(username);
                    returnUserVm.Token = tokenStr;
                    //返回用户详细信息
                    returnUserVm.Id = users.Id;
                    returnUserVm.UserName = users.UserName;
                    returnUserVm.ShowName = users.ShowName;
                    returnUserVm.ImgUrl = ImageUrlHelper.GetHeadImageUrl(users.ImgUrl, 180);
                    returnUserVm.PhoneNum = users.PhoneNum;
                    returnUserVm.Email = users.Email;
                    returnUserVm.Sex = users.Sex? 1:0;
                }
            }
            else
            {
                description = "用户名或密码不能为空";
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = isSuccess ? 1 : 0,
                Data = returnUserVm,
                Description = description
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage LoginOAoth([FromBody]LoginOauthViewModel loginViewModel)
        {
            var isSuccess = false;
            var description = string.Empty;
            var token = string.Empty;
            if (!string.IsNullOrEmpty(loginViewModel.OpenId) && !string.IsNullOrEmpty(loginViewModel.OauthType))
            {
                var users = _userBll.GetModelByUserName(loginViewModel.OpenId);
                if (users == null || !loginViewModel.OauthType.Equals(users.OAuthType))
                {//不存在， 创建用户
                    var model = new Users
                    {
                        UserName = loginViewModel.OpenId,
                        PassWord = loginViewModel.OauthType,
                        OAuthType = loginViewModel.OauthType,
                        ShowName = loginViewModel.ShowName,
                        ImgUrl = loginViewModel.HeadImage,
                        IsEnable = true
                    };
                    var userid = _userBll.Add(model);
                    users = _userBll.GetModelByCache(userid);
                }
                var tokenStr = TokenHelper.AddOrUpdateToken(users.UserName);
                token = tokenStr;
                isSuccess = true;
            }
            else
            {
                description = "数据不完整";
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = isSuccess ? 1 : 0,
                Data = token,
                Description = description
            });
        }

        /// <summary>
        /// 用手机+验证码登录（如未注册，则添加新用户）
        /// </summary>
        /// <returns>登录成功，返回token</returns>
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage LoginByPhone([FromBody]LoginByPhoneViewModel vm)
        {
            var flag = PhoneCheckNumBll.CheckPhoneNum(vm.PhoneNum, vm.CheckNum, SendCheckNumOperateEnum.LoginByPhone);
            if (flag)
            {
                var isExists = _userBll.GetModelByPhoneNum(vm.PhoneNum);
                if (isExists == null)
                {
                    //添加新用户
                    var user = new Users
                    {
                        PhoneNum = vm.PhoneNum,
                        UserName = vm.PhoneNum
                    };
                    var userid = _userBll.Add(user);
                    if (userid > 0)
                    {
                        return GetJson(new JsonResultViewModel
                        {
                            IsSuccess = 1,
                            Description = "登录成功",
                            Data = ""
                        });
                    }
                }
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Description = "登录失败",
                Data = ""
            });


        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Regist([FromBody]UserRegistViewModel vm)
        {
            var flag = PhoneCheckNumBll.CheckPhoneNum(vm.PhoneNum, vm.CheckNum, SendCheckNumOperateEnum.Regist);
            if (flag)
            {
                if (vm.Password.Equals(vm.Password2))
                {
                    //添加新用户
                    var user = new Users
                    {
                        PhoneNum = vm.PhoneNum,
                        PassWord = vm.Password,
                        UserName = vm.Username
                    };
                    var newid = _userBll.Add(user);
                    if (newid > 0)
                    {
                        return GetJson(new JsonResultViewModel
                        {
                            IsSuccess = 1,
                            Description = "注册成功",
                            Data = ""
                        });
                    }
                }
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Description = "注册失败",
                Data = ""
            });
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage FindPassword([FromBody]FindPasswordViewModel vm)
        {
            var flag = PhoneCheckNumBll.CheckPhoneNum(vm.PhoneNum, vm.CheckNum, SendCheckNumOperateEnum.FindPassword);
            if (flag)
            {
                if (!string.IsNullOrEmpty(vm.Password) && vm.Password.Length>6 && vm.Password.Equals(vm.Password2))
                {
                    var user = _userBll.GetModelByPhoneNum(vm.PhoneNum);
                    user.PassWord = vm.Password;
                    user.UpdateTime = DateTime.Now;
                    var flagupdate = _userBll.Update(user);
                    if (flagupdate)
                    {
                        return GetJson(new JsonResultViewModel
                        {
                            IsSuccess = 1,
                            Description = "找回密码成功",
                            Data = ""
                        });
                    }
                }
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 0,
                    Description = "密码不能为空",
                    Data = new { step=1,newPass="请输入新密码,长度不少于6位"}
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Description = "验证码超时或无效",
                Data = ""
            });
        }


        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SendCheckNum([FromBody]SendCheckNumViewModel vm)
        {
            switch ((SendCheckNumOperateEnum)vm.OpreateType)
            {
                case SendCheckNumOperateEnum.Regist:
                    var user = _userBll.GetModelByPhoneNum(vm.PhoneNum);
                    if (user != null)
                    {
                        return GetJson(new JsonResultViewModel
                        {
                            IsSuccess = 0,
                            Description = "该手机号已注册，请直接登录",
                            Data = DateTime.Now.ToLongTimeString()
                        });
                    }
                    break;
                case SendCheckNumOperateEnum.FindPassword:
                    var userfp = _userBll.GetModelByPhoneNum(vm.PhoneNum);
                    if (userfp == null)
                    {
                        return GetJson(new JsonResultViewModel
                        {
                            IsSuccess = 0,
                            Description = "该手机号不存在",
                            Data = DateTime.Now.ToLongTimeString()
                        });
                    }
                    break;
            }
            var operateEnum = (SendCheckNumOperateEnum)vm.OpreateType;
            var flag = PhoneCheckNumBll.Send(vm.PhoneNum, operateEnum);
            if (flag)
            {
                return GetJson(new JsonResultViewModel
                {
                    IsSuccess = 1,
                    Description = "验证码发送成功",
                    Data = DateTime.Now.ToLongTimeString()
                });
            }
            return GetJson(new JsonResultViewModel
            {
                IsSuccess = 0,
                Description = "验证码已发送，请1分钟后再试",
                Data = ""
            });
        }
    }
}
