# -*- coding: utf-8 -*-

import re
from django.contrib.auth.models import User
from django.http import JsonResponse, Http404, HttpResponse
from django.views.generic.base import View
from django.contrib.auth import password_validation, authenticate
from django.core.exceptions import ValidationError

class RegisterView(View):
    usernameregex = re.compile(r'^[\w.@+-]+$')
    def get(self, request):
        if ('HTTP_USER_AGENT' in request.META and
            request.META['HTTP_USER_AGENT'] == 'Unity'):
            username = request.GET.get('username', '')
            password = request.GET.get('password', '')
            errors = []
            if len(username) > 150:
                errors.append('Username is too long, max length is 150 symbols')
            if not self.usernameregex.match(username):
                errors.append('Enter a valid username. This value may contain only letters, numbers, and @/./+/-/_ characters.')
            if User.objects.filter(username=username).exists():
                errors.append('A user with that username already exists.')
            if not errors:
                user = User.objects.create_user(username)
                try:
                    password_validation.validate_password(password, user=user)
                except ValidationError as e:
                    user.delete()
                    errors.extend(list(e))
                if not errors:
                    user.set_password(password)
                    user.save()
                    return JsonResponse({'status' : 'success', 'errors' : ''})
            errorstring = ';'.join(errors)
            return JsonResponse({'status' : 'error', 'errors' : errorstring})
        else:
            raise Http404()

class LoginView(View):
    def get(self, request):
        if ('HTTP_USER_AGENT' in request.META and
            request.META['HTTP_USER_AGENT'] == 'Unity'):
            username = request.GET.get('username', '')
            password = request.GET.get('password', '')
            user = authenticate(username=username, password=password)
            if user is not None:
                if user.is_active:
                    return JsonResponse({'status' : 'success', 'errors' : ''})
                else:
                    return JsonResponse({'status' : 'error', 'errors' : 'This user is inactive'})
            else:
                return JsonResponse({'status' : 'error', 'errors' : 'Wrong password or no such user'})
        else:
            raise Http404()
