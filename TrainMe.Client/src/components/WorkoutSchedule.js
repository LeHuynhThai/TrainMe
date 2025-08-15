import React, { useState, useEffect } from 'react';
import { workoutItemAPI } from '../services/api';

const WorkoutSchedule = () => {
  const [workouts, setWorkouts] = useState({
    1: [], // Monday
    2: [], // Tuesday
    3: [], // Wednesday
    4: [], // Thursday
    5: [], // Friday
    6: [], // Saturday
    7: []  // Sunday
  });

  const [newWorkout, setNewWorkout] = useState('');
  const [selectedDay, setSelectedDay] = useState(1); // Use numeric values for API
  const [editingWorkout, setEditingWorkout] = useState(null);
  const [editingText, setEditingText] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const daysOfWeek = [
    { key: 1, name: 'Thứ 2', color: '#ef4444' },
    { key: 2, name: 'Thứ 3', color: '#f97316' },
    { key: 3, name: 'Thứ 4', color: '#eab308' },
    { key: 4, name: 'Thứ 5', color: '#22c55e' },
    { key: 5, name: 'Thứ 6', color: '#3b82f6' },
    { key: 6, name: 'Thứ 7', color: '#8b5cf6' },
    { key: 7, name: 'Chủ nhật', color: '#ec4899' }
  ];

  // Load workouts from API
  useEffect(() => {
    loadWorkouts();
  }, []);

  // Load workouts from backend API
  const loadWorkouts = async () => {
    try {
      setLoading(true);
      setError('');
      const response = await workoutItemAPI.getWorkoutItemsGrouped();

      if (response.data.success) {
        setWorkouts(response.data.data);
      } else {
        setError(response.data.message || 'Không thể tải dữ liệu');
      }
    } catch (err) {
      console.error('Error loading workouts:', err);
      setError('Lỗi kết nối đến server');
    } finally {
      setLoading(false);
    }
  };

  // Add new workout via API
  const addWorkout = async () => {
    if (newWorkout.trim() === '') return;

    try {
      setLoading(true);
      setError('');

      const workoutData = {
        name: newWorkout.trim(),
        dayOfWeek: selectedDay,
        sortOrder: 0
      };

      const response = await workoutItemAPI.createWorkoutItem(workoutData);

      if (response.data.success) {
        // Reload workouts to get updated data
        await loadWorkouts();
        setNewWorkout('');
      } else {
        setError(response.data.message || 'Không thể thêm bài tập');
      }
    } catch (err) {
      console.error('Error adding workout:', err);
      setError('Lỗi khi thêm bài tập');
    } finally {
      setLoading(false);
    }
  };

  // Delete workout via API
  const deleteWorkout = async (day, workoutId) => {
    try {
      setLoading(true);
      setError('');

      const response = await workoutItemAPI.deleteWorkoutItem(workoutId);

      if (response.data.success) {
        // Reload workouts to get updated data
        await loadWorkouts();
      } else {
        setError(response.data.message || 'Không thể xóa bài tập');
      }
    } catch (err) {
      console.error('Error deleting workout:', err);
      setError('Lỗi khi xóa bài tập');
    } finally {
      setLoading(false);
    }
  };

  // Start editing workout
  const startEdit = (day, workout) => {
    setEditingWorkout({ day, id: workout.id });
    setEditingText(workout.name);
  };

  // Save edited workout via API
  const saveEdit = async () => {
    if (editingText.trim() === '') return;

    try {
      setLoading(true);
      setError('');

      const updateData = {
        name: editingText.trim(),
        dayOfWeek: editingWorkout.day,
        sortOrder: 0
      };

      const response = await workoutItemAPI.updateWorkoutItem(editingWorkout.id, updateData);

      if (response.data.success) {
        // Reload workouts to get updated data
        await loadWorkouts();
        setEditingWorkout(null);
        setEditingText('');
      } else {
        setError(response.data.message || 'Không thể cập nhật bài tập');
      }
    } catch (err) {
      console.error('Error updating workout:', err);
      setError('Lỗi khi cập nhật bài tập');
    } finally {
      setLoading(false);
    }
  };

  // Cancel editing
  const cancelEdit = () => {
    setEditingWorkout(null);
    setEditingText('');
  };

  // Calculate total workouts
  const getTotalWorkouts = () => {
    return Object.values(workouts).reduce((total, dayWorkouts) => total + (dayWorkouts?.length || 0), 0);
  };

  return (
    <div className="workout-schedule">
      <div className="workout-header">
        <h1>Lịch Tập Thể Dục</h1>
        <div className="workout-stats">
          <span className="total-workouts">
            Tổng số bài tập: <strong>{getTotalWorkouts()}</strong>
          </span>
          {loading && <span className="loading">Đang tải...</span>}
        </div>
      </div>

      {/* Error Message */}
      {error && (
        <div className="error-message" style={{
          backgroundColor: '#fee2e2',
          color: '#dc2626',
          padding: '12px',
          borderRadius: '6px',
          marginBottom: '16px',
          border: '1px solid #fecaca'
        }}>
          {error}
        </div>
      )}

      {/* Add Workout Form */}
      <div className="add-workout-form">
        <div className="form-row">
          <select
            value={selectedDay}
            onChange={(e) => setSelectedDay(e.target.value)}
            className="day-select"
          >
            {daysOfWeek.map(day => (
              <option key={day.key} value={day.key}>
                {day.name}
              </option>
            ))}
          </select>
          
          <input
            type="text"
            value={newWorkout}
            onChange={(e) => setNewWorkout(e.target.value)}
            placeholder="Nhập tên bài tập..."
            className="workout-input"
            onKeyPress={(e) => e.key === 'Enter' && addWorkout()}
          />
          
          <button
            onClick={addWorkout}
            className="btn btn-primary add-btn"
            disabled={!newWorkout.trim() || loading}
          >
            {loading ? 'Đang thêm...' : 'Thêm bài tập'}
          </button>
        </div>
      </div>

      {/* Weekly Schedule */}
      <div className="schedule-container">
        <div className="schedule-hint">
          <p>💡 Vuốt ngang để xem tất cả các ngày trong tuần</p>
        </div>
        <div className="weekly-schedule">
        {daysOfWeek.map(day => (
          <div key={day.key} className="day-column">
            <div 
              className="day-header"
              style={{ backgroundColor: day.color }}
            >
              <h3>{day.name}</h3>
              <span className="workout-count">
                {workouts[day.key]?.length || 0} bài tập
              </span>
            </div>
            
            <div className="workout-list">
              {(workouts[day.key] || []).map(workout => (
                <div key={workout.id} className="workout-item">
                  {editingWorkout?.day === day.key && editingWorkout?.id === workout.id ? (
                    <div className="edit-form">
                      <input
                        type="text"
                        value={editingText}
                        onChange={(e) => setEditingText(e.target.value)}
                        className="edit-input"
                        onKeyPress={(e) => e.key === 'Enter' && saveEdit()}
                        autoFocus
                      />
                      <div className="edit-actions">
                        <button onClick={saveEdit} className="btn-save">✓</button>
                        <button onClick={cancelEdit} className="btn-cancel">✕</button>
                      </div>
                    </div>
                  ) : (
                    <div className="workout-content">
                      <span className="workout-name">{workout.name}</span>
                      <div className="workout-actions">
                        <button
                          onClick={() => startEdit(day.key, workout)}
                          className="btn-edit"
                          title="Sửa bài tập"
                        >
                          ✏️
                        </button>
                        <button
                          onClick={() => deleteWorkout(day.key, workout.id)}
                          className="btn-delete"
                          title="Xóa bài tập"
                        >
                          🗑️
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              ))}
              
              {(!workouts[day.key] || workouts[day.key].length === 0) && (
                <div className="empty-day">
                  <p>Chưa có bài tập nào</p>
                </div>
              )}
            </div>
          </div>
        ))}
        </div>
      </div>
    </div>
  );
};

export default WorkoutSchedule;
