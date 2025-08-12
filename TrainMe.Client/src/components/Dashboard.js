import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

const Dashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);

  const handleLogout = async () => {
    setLoading(true);
    try {
      await logout();
      navigate('/login');
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'Chưa có thông tin';
    return new Date(dateString).toLocaleDateString('vi-VN', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  return (
    <div style={{
      minHeight: '100vh',
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'
    }}>
      {/* Header */}
      <div style={{
        background: 'rgba(255, 255, 255, 0.1)',
        backdropFilter: 'blur(10px)',
        borderBottom: '1px solid rgba(255, 255, 255, 0.2)',
        padding: '16px 0'
      }}>
        <div className="container" style={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center'
        }}>
          <div>
            <h1 style={{
              color: 'white',
              margin: 0,
              fontSize: '32px',
              fontWeight: '700'
            }}>
              🚀 TrainMe Dashboard
            </h1>
            <p style={{
              color: 'rgba(255, 255, 255, 0.8)',
              margin: '4px 0 0 0'
            }}>
              Chào mừng trở lại, {user?.userName}!
            </p>
          </div>

          <button
            onClick={handleLogout}
            disabled={loading}
            className={`btn btn-danger ${loading ? 'loading' : ''}`}
            style={{ fontSize: '16px' }}
          >
            {loading ? (
              <>
                <span style={{ marginRight: '8px' }}>⏳</span>
                Đang đăng xuất...
              </>
            ) : (
              <>
                <span style={{ marginRight: '8px' }}>🚪</span>
                Đăng xuất
              </>
            )}
          </button>
        </div>
      </div>

      {/* Main Content */}
      <div className="container" style={{ padding: '40px 20px' }}>
        <div style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
          gap: '24px',
          marginBottom: '32px'
        }}>
          {/* User Info Card */}
          <div className="card">
            <div className="card-header" style={{ background: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)' }}>
              <h3 style={{ margin: 0, fontSize: '20px', fontWeight: '600' }}>
                👤 Thông tin tài khoản
              </h3>
            </div>
            <div className="card-body">
              <div style={{ marginBottom: '16px' }}>
                <strong style={{ color: '#495057' }}>Tên đăng nhập:</strong>
                <p style={{ margin: '4px 0 0 0', fontSize: '18px', fontWeight: '500' }}>
                  {user?.userName}
                </p>
              </div>

              <div style={{ marginBottom: '16px' }}>
                <strong style={{ color: '#495057' }}>Vai trò:</strong>
                <span style={{
                  display: 'inline-block',
                  marginLeft: '8px',
                  padding: '4px 12px',
                  background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                  color: 'white',
                  borderRadius: '16px',
                  fontSize: '14px',
                  fontWeight: '500'
                }}>
                  {user?.role}
                </span>
              </div>

              <div>
                <strong style={{ color: '#495057' }}>Ngày tạo tài khoản:</strong>
                <p style={{ margin: '4px 0 0 0', color: '#6c757d' }}>
                  {formatDate(user?.createdAt)}
                </p>
              </div>
            </div>
          </div>

          {/* Stats Card */}
          <div className="card">
            <div className="card-header" style={{ background: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)' }}>
              <h3 style={{ margin: 0, fontSize: '20px', fontWeight: '600' }}>
                📊 Thống kê
              </h3>
            </div>
            <div className="card-body">
              <div style={{ textAlign: 'center', padding: '20px 0' }}>
                <div style={{ fontSize: '48px', marginBottom: '16px' }}>🎯</div>
                <h4 style={{ margin: '0 0 8px 0', color: '#495057' }}>
                  Chào mừng đến với TrainMe!
                </h4>
                <p style={{ color: '#6c757d', margin: 0 }}>
                  Hệ thống đang được phát triển...
                </p>
              </div>
            </div>
          </div>

          {/* Quick Actions Card */}
          <div className="card">
            <div className="card-header" style={{ background: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)' }}>
              <h3 style={{ margin: 0, fontSize: '20px', fontWeight: '600' }}>
                ⚡ Thao tác nhanh
              </h3>
            </div>
            <div className="card-body">
              <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                <button className="btn btn-secondary" style={{ justifyContent: 'flex-start' }}>
                  <span style={{ marginRight: '8px' }}>📝</span>
                  Cập nhật thông tin
                </button>
                <button className="btn btn-secondary" style={{ justifyContent: 'flex-start' }}>
                  <span style={{ marginRight: '8px' }}>🔒</span>
                  Đổi mật khẩu
                </button>
                <button className="btn btn-secondary" style={{ justifyContent: 'flex-start' }}>
                  <span style={{ marginRight: '8px' }}>⚙️</span>
                  Cài đặt
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Welcome Message */}
        <div className="card">
          <div className="card-body" style={{ textAlign: 'center', padding: '40px' }}>
            <div style={{ fontSize: '64px', marginBottom: '24px' }}>🎉</div>
            <h2 style={{ margin: '0 0 16px 0', color: '#495057' }}>
              Chào mừng bạn đến với TrainMe!
            </h2>
            <p style={{ color: '#6c757d', fontSize: '18px', maxWidth: '600px', margin: '0 auto' }}>
              Đây là trang dashboard của bạn. Hệ thống đang được phát triển và sẽ có thêm nhiều tính năng thú vị trong tương lai.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
