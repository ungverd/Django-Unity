# -*- coding: utf-8 -*-

from django.shortcuts import render

from django.views.generic.edit import FormView
from django.contrib.auth.forms import UserCreationForm
from django import forms

class RegisterFormView(FormView):
    form_class = UserCreationForm

    success_url = "/login/"

    template_name = "register.html"

    def form_valid(self, form):
        form.save()
        return super(RegisterFormView, self).form_valid(form)

from django.contrib.auth.forms import AuthenticationForm

from django.contrib.auth import login

class LoginFormView(FormView):
    form_class = AuthenticationForm

    template_name = "login.html"
    success_url = "/"
            
    def form_valid(self, form):
        self.user = form.get_user()
        login(self.request, self.user)
        return super(LoginFormView, self).form_valid(form)

from django.http import HttpResponseRedirect
from django.views.generic.base import View
from django.contrib.auth import logout

class LogoutView(View):
    def get(self, request):
        logout(request)
        return HttpResponseRedirect("/")

def index(request):
    return render(request, 'index.html', {'user': request.user})
