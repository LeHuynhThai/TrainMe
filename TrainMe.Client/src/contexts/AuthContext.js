import React, { createContext, useContext, useState, useEffect } from 'react';
import { authAPI } from '../services/api';

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      authAPI.getCurrentUser()
        .then(response => {
          if (response.data.success) {
            setUser(response.data.data);
          }
        })
        .catch(() => {
          localStorage.removeItem('token');
        })
        .finally(() => {
          setLoading(false);
        });
    } else {
      setLoading(false);
    }
  }, []);

  const login = async (userName, password) => {
    try {
      console.log('Attempting login with:', { userName, password });
      const response = await authAPI.login({ userName, password });
      console.log('Login response:', response);

      if (response.data.success) {
        const authData = response.data.data;
        const { accessToken, user: userData } = authData;
        localStorage.setItem('token', accessToken);
        setUser(userData);
        return { success: true, data: authData };
      } else {
        console.log('Login failed:', response.data);
        return { success: false, message: response.data.message };
      }
    } catch (error) {
      console.error('Login error:', error);
      const message = error.response?.data?.message || 'Đăng nhập thất bại';
      return { success: false, message };
    }
  };

  const register = async (userName, password) => {
    try {
      console.log('Attempting register with:', { userName, password });
      const response = await authAPI.register({ userName, password });
      console.log('Register response:', response);

      if (response.data.success) {
        return { success: true, data: response.data.data };
      } else {
        console.log('Register failed:', response.data);
        return { success: false, message: response.data.message, errors: response.data.errors };
      }
    } catch (error) {
      console.error('Register error:', error);
      const message = error.response?.data?.message || 'Đăng ký thất bại';
      const errors = error.response?.data?.errors || [];
      return { success: false, message, errors };
    }
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  const value = {
    user,
    loading,
    login,
    register,
    logout
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};
