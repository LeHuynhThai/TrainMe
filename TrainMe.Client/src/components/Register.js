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

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    // Validation
    if (formData.password !== formData.confirmPassword) {
      setError('Mật khẩu xác nhận không khớp');
      return;
    }

    if (formData.password.length < 6) {
      setError('Mật khẩu phải có ít nhất 6 ký tự');
      return;
    }

    setLoading(true);

    try {
      const result = await register(formData.userName, formData.password);
      if (result.success) {
        setSuccess('Đăng ký thành công! Đang chuyển đến trang đăng nhập...');
        setTimeout(() => {
          navigate('/login');
        }, 2000);
      } else {
        setError(result.message || 'Đăng ký thất bại');
      }
    } catch (err) {
      setError('Có lỗi xảy ra khi đăng ký');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container" style={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      minHeight: '100vh',
      padding: '20px'
    }}>
      <div className="card" style={{ width: '100%', maxWidth: '400px' }}>
        <div className="card-header">
          <h2 style={{ margin: 0, fontSize: '28px', fontWeight: '700' }}>
            🎯 Tham gia TrainMe
          </h2>
          <p style={{ margin: '8px 0 0 0', opacity: 0.9 }}>
            Tạo tài khoản mới để bắt đầu
          </p>
        </div>

        <div className="card-body">
          {error && (
            <div className="alert alert-danger">
              <strong>Lỗi!</strong> {error}
            </div>
          )}

          {success && (
            <div className="alert alert-success">
              <strong>Thành công!</strong> {success}
            </div>
          )}

          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label className="form-label">
                👤 Tên đăng nhập
              </label>
              <input
                type="text"
                name="userName"
                value={formData.userName}
                onChange={handleChange}
                required
                className="form-control"
                placeholder="Chọn tên đăng nhập của bạn"
                autoComplete="username"
                minLength="3"
              />
            </div>

            <div className="form-group">
              <label className="form-label">
                🔒 Mật khẩu
              </label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
                className="form-control"
                placeholder="Tạo mật khẩu mạnh (ít nhất 6 ký tự)"
                autoComplete="new-password"
                minLength="6"
              />
            </div>

            <div className="form-group">
              <label className="form-label">
                🔐 Xác nhận mật khẩu
              </label>
              <input
                type="password"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
                required
                className="form-control"
                placeholder="Nhập lại mật khẩu để xác nhận"
                autoComplete="new-password"
              />
            </div>

            <button
              type="submit"
              disabled={loading}
              className={`btn btn-primary ${loading ? 'loading' : ''}`}
              style={{ width: '100%', fontSize: '16px', padding: '14px' }}
            >
              {loading ? (
                <>
                  <span style={{ marginRight: '8px' }}>⏳</span>
                  Đang đăng ký...
                </>
              ) : (
                <>
                  <span style={{ marginRight: '8px' }}>🎯</span>
                  Đăng ký tài khoản
                </>
              )}
            </button>
          </form>

          <div className="text-center mt-3">
            <p style={{ color: '#6c757d', marginBottom: '16px' }}>
              Đã có tài khoản?
            </p>
            <Link
              to="/login"
              className="btn btn-secondary"
              style={{ textDecoration: 'none' }}
            >
              <span style={{ marginRight: '8px' }}>🚪</span>
              Đăng nhập ngay
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;
