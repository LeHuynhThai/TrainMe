import axios from 'axios';

// Tạo axios instance với base URL
const api = axios.create({
  baseURL: 'http://localhost:5178/api',
  headers: { 'Content-Type': 'application/json' },
});

// Request Interceptor - Tự động thêm JWT token vào mọi request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response Interceptor - Tự động logout khi token hết hạn
api.interceptors.response.use(
  (response) => response,
  (error) => {
    // Nếu server trả về 401 (Unauthorized)
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Auth API endpoints
export const authAPI = {
  // Đăng ký tài khoản
  register: (data) => api.post('/auth/register', data),
  
  // Đăng nhập
  login: (data) => api.post('/auth/login', data),
  
  // Lấy thông tin user hiện tại
  getCurrentUser: () => api.get('/auth/me'),
};

export default api;
