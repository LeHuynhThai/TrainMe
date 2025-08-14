import React, { useState, useEffect } from 'react';

const WorkoutSchedule = () => {
  const [workouts, setWorkouts] = useState({
    monday: [],
    tuesday: [],
    wednesday: [],
    thursday: [],
    friday: [],
    saturday: [],
    sunday: []
  });

  const [newWorkout, setNewWorkout] = useState('');
  const [selectedDay, setSelectedDay] = useState('monday');
  const [editingWorkout, setEditingWorkout] = useState(null);
  const [editingText, setEditingText] = useState('');

  const daysOfWeek = [
    { key: 'monday', name: 'Thứ 2', color: '#ef4444' },
    { key: 'tuesday', name: 'Thứ 3', color: '#f97316' },
    { key: 'wednesday', name: 'Thứ 4', color: '#eab308' },
    { key: 'thursday', name: 'Thứ 5', color: '#22c55e' },
    { key: 'friday', name: 'Thứ 6', color: '#3b82f6' },
    { key: 'saturday', name: 'Thứ 7', color: '#8b5cf6' },
    { key: 'sunday', name: 'Chủ nhật', color: '#ec4899' }
  ];

  // Load workouts from localStorage
  useEffect(() => {
    const savedWorkouts = localStorage.getItem('workoutSchedule');
    if (savedWorkouts) {
      setWorkouts(JSON.parse(savedWorkouts));
    }
  }, []);

  // Save workouts to localStorage
  useEffect(() => {
    localStorage.setItem('workoutSchedule', JSON.stringify(workouts));
  }, [workouts]);

  // Add new workout
  const addWorkout = () => {
    if (newWorkout.trim() === '') return;

    const workout = {
      id: Date.now(),
      name: newWorkout.trim()
    };

    setWorkouts(prev => ({
      ...prev,
      [selectedDay]: [...prev[selectedDay], workout]
    }));

    setNewWorkout('');
  };

  // Delete workout
  const deleteWorkout = (day, workoutId) => {
    setWorkouts(prev => ({
      ...prev,
      [day]: prev[day].filter(workout => workout.id !== workoutId)
    }));
  };

  // Start editing workout
  const startEdit = (day, workout) => {
    setEditingWorkout({ day, id: workout.id });
    setEditingText(workout.name);
  };

  // Save edited workout
  const saveEdit = () => {
    if (editingText.trim() === '') return;

    setWorkouts(prev => ({
      ...prev,
      [editingWorkout.day]: prev[editingWorkout.day].map(workout =>
        workout.id === editingWorkout.id
          ? { ...workout, name: editingText.trim() }
          : workout
      )
    }));

    setEditingWorkout(null);
    setEditingText('');
  };

  // Cancel editing
  const cancelEdit = () => {
    setEditingWorkout(null);
    setEditingText('');
  };

  // Calculate total workouts
  const getTotalWorkouts = () => {
    return Object.values(workouts).reduce((total, dayWorkouts) => total + dayWorkouts.length, 0);
  };

  return (
    <div className="workout-schedule">
      <div className="workout-header">
        <h1>Quản Lý Bài Tập Thể Dục</h1>
        <div className="workout-stats">
          <span className="total-workouts">
            Tổng số bài tập: <strong>{getTotalWorkouts()}</strong>
          </span>
        </div>
      </div>

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
            disabled={!newWorkout.trim()}
          >
            Thêm bài tập
          </button>
        </div>
      </div>

      {/* Weekly Schedule */}
      <div className="weekly-schedule">
        {daysOfWeek.map(day => (
          <div key={day.key} className="day-column">
            <div 
              className="day-header"
              style={{ backgroundColor: day.color }}
            >
              <h3>{day.name}</h3>
              <span className="workout-count">
                {workouts[day.key].length} bài tập
              </span>
            </div>
            
            <div className="workout-list">
              {workouts[day.key].map(workout => (
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
              
              {workouts[day.key].length === 0 && (
                <div className="empty-day">
                  <p>Chưa có bài tập nào</p>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default WorkoutSchedule;
