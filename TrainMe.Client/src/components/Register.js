import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const Register = () => {
  const navigate = useNavigate();
  const { register } = useAuth();
  
  const [formData, setFormData] = useState({
    userName: '',
    password: '',
    confirmPassword: ''
  });
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    // Validation
    if (formData.password !== formData.confirmPassword) {
      setError('Mật khẩu xác nhận không khớp');
      return;
    }

    if (formData.password.length < 3 || formData.password.length > 8) {
      setError('Mật khẩu phải có từ 3-8 ký tự');
      return;
    }

    setLoading(true);

    const result = await register(formData.userName, formData.password);
    
    if (result.success) {
      setSuccess('Đăng ký thành công! Đang chuyển đến trang đăng nhập...');
      setTimeout(() => {
        navigate('/login');
      }, 1000);
    } else {
      setError(result.message);
    }
    
    setLoading(false);
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  return (
    <div className="auth-page">
      {/* Header */}
      <header className="auth-header">
        <div className="auth-header-content">
          <div className="header-logo">
            <div className="logo-icon">TM</div>
            <span className="logo-text">TrainMe</span>
          </div>
          <nav className="nav-menu">
            <span className="nav-item">Dashboard</span>
            <span className="nav-item">Courses</span>
            <span className="nav-item">Progress</span>
            <span className="nav-item">About</span>
          </nav>
        </div>
      </header>

      {/* Main Content */}
      <div className="auth-content">
        <div className="auth-card">
          <div className="auth-card-header">
            <h2>Đăng ký</h2>
            <p>Tạo tài khoản TrainMe mới</p>
          </div>

          {error && (
            <div className="alert alert-danger">
              <strong>Lỗi:</strong> {error}
            </div>
          )}

          {success && (
            <div className="alert alert-success">
              <strong>Thành công:</strong> {success}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label className="form-label">Tên đăng nhập</label>
              <input
                type="text"
                name="userName"
                className="form-control"
                placeholder="Nhập tên đăng nhập (tối thiểu 3 ký tự)"
                value={formData.userName}
                onChange={handleChange}
                required
                minLength="3"
                maxLength="100"
              />
            </div>

            <div className="form-group">
              <label className="form-label">Mật khẩu</label>
              <input
                type="password"
                name="password"
                className="form-control"
                placeholder="Nhập mật khẩu (3-8 ký tự)"
                value={formData.password}
                onChange={handleChange}
                required
                minLength="3"
                maxLength="8"
              />
            </div>

            <div className="form-group">
              <label className="form-label">Xác nhận mật khẩu</label>
              <input
                type="password"
                name="confirmPassword"
                className="form-control"
                placeholder="Nhập lại mật khẩu"
                value={formData.confirmPassword}
                onChange={handleChange}
                required
                minLength="3"
                maxLength="8"
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className={`btn btn-primary ${loading ? 'loading' : ''}`}
            >
              {loading ? 'Đang đăng ký...' : 'Đăng ký'}
            </button>
          </form>

          <div className="auth-footer">
            <p className="text-muted">Đã có tài khoản?</p>
            <Link to="/login" className="btn btn-secondary">
              Đăng nhập ngay
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;
