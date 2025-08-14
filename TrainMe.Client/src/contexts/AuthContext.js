import React, { createContext, useContext, useState, useEffect } from 'react';
import { authAPI } from '../services/api';

// Tạo Context
const AuthContext = createContext();

// Custom hook để sử dụng AuthContext
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

// AuthProvider component
export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  // Kiểm tra user đã login chưa khi app khởi động
  useEffect(() => {
    const initializeAuth = async () => {
      const token = localStorage.getItem('token');
      if (token) {
        try {
          const response = await authAPI.getCurrentUser();
          if (response.data.success) {
            setUser(response.data.data);
          }
        } catch (error) {
          localStorage.removeItem('token');
        }
      }
      setLoading(false);
    };

    initializeAuth();
  }, []);

  // Hàm đăng nhập
  const login = async (userName, password) => {
    try {
      const response = await authAPI.login({ userName, password });
      
      if (response.data.success) {
        const { accessToken, user: userData } = response.data.data;
        localStorage.setItem('token', accessToken);
        setUser(userData);
        return { success: true };
      }
      
      return { success: false, message: response.data.message };
    } catch (error) {
      return { 
        success: false, 
        message: error.response?.data?.message || 'Đăng nhập thất bại' 
      };
    }
  };

  // Hàm đăng ký
  const register = async (userName, password) => {
    try {
      const response = await authAPI.register({ userName, password });
      
      return response.data.success 
        ? { success: true } 
        : { success: false, message: response.data.message };
    } catch (error) {
      return { 
        success: false, 
        message: error.response?.data?.message || 'Đăng ký thất bại' 
      };
    }
  };

  // Hàm đăng xuất
  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  const value = {
    user,
    login,
    register,
    logout,
    loading
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};
