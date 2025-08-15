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

// WorkoutItem API endpoints
export const workoutItemAPI = {
  // Lấy tất cả workout items của user hiện tại
  getMyWorkoutItems: () => api.get('/workoutitems'),

  // Lấy workout items theo ngày
  getWorkoutItemsByDay: (dayOfWeek) => api.get(`/workoutitems/day/${dayOfWeek}`),

  // Lấy workout items grouped by day
  getWorkoutItemsGrouped: () => api.get('/workoutitems/grouped'),

  // Lấy workout item theo ID
  getWorkoutItem: (id) => api.get(`/workoutitems/${id}`),

  // Tạo workout item mới
  createWorkoutItem: (data) => api.post('/workoutitems', data),

  // Cập nhật workout item
  updateWorkoutItem: (id, data) => api.put(`/workoutitems/${id}`, data),

  // Xóa workout item
  deleteWorkoutItem: (id) => api.delete(`/workoutitems/${id}`),

  // Sắp xếp lại workout items
  reorderWorkoutItems: (data) => api.put('/workoutitems/reorder', data),

  // Duplicate workout item sang ngày khác
  duplicateWorkoutItem: (id, data) => api.post(`/workoutitems/${id}/duplicate`, data),
};

export default api;
