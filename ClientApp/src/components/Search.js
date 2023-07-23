import React, { Component } from 'react';

class Search extends Component {
  constructor(props) {
    super(props);

    this.state = {
      filters: [],
    };
  }

  componentDidMount() {
    const urlFilters = document.location.pathname //this.props.match.params; TODO learn react-router-dom

    const segments = urlFilters.split('/').filter((str) => str !== '')

    if (segments.length) {
      const parsedFilters = segments
          .reduce((result, value, index, array) => {
              if (index % 3 === 0) {
                result.push({ activity: value, action: array[index + 1], time: array[index + 2] });
              }
              return result;
      }, []);

      this.setState({ filters: parsedFilters });
    }
  }

  handleActivityChange = (index, value) => {
    const { filters } = this.state;
    const updatedFilters = [...filters];
    updatedFilters[index].activity = value;
    this.setState({ filters: updatedFilters });
  };

  handleActionChange = (index, value) => {
    const { filters } = this.state;
    const updatedFilters = [...filters];
    updatedFilters[index].action = value;
    this.setState({ filters: updatedFilters });
  };

  handleTimeChange = (index, value) => {
    const { filters } = this.state;
    const updatedFilters = [...filters];
    updatedFilters[index].time = value;
    this.setState({ filters: updatedFilters });
  };

  handleAddFilter = () => {
    const { filters } = this.state;
    this.setState({ filters: [...filters, { activity: '', action: '', time: '' }] });
  };

  handleRemoveFilter = (index) => {
    const { filters } = this.state;
    const updatedFilters = [...filters];
    updatedFilters.splice(index, 1);
    this.setState({ filters: updatedFilters });
  };

  handleSubmit = () => {
    const { filters } = this.state;
    const url = filters
      .map((filter) => `${filter.activity}/${filter.action}/${filter.time}`)
      .join('/');
    
      document.location.pathname = url; // this.props.history.push(url); TODO: LEARN REACT ROUTER 
  };

  render() {
    const { filters } = this.state;

    return (
      <div>
        {filters.map((filter, index) => (
          <div class="form-group row"  key={index}>
            <select class="col-4" value={filter.activity} onChange={(e) => this.handleActivityChange(index, e.target.value)}>
              <option value="">Activity</option>
              <option value="NightTime">NightTime</option>
              <option value="Awake">Awake</option>
              <option value="Nap">Nap</option>
            </select>
            <select class="col-3" value={filter.action} onChange={(e) => this.handleActionChange(index, e.target.value)}>
              <option value="">Action</option>
              <option value="Starts">Starts</option>
              <option value="Ends">Ends</option>
            </select>
            <input  class="col-3" type="text" value={filter.time} onChange={(e) => this.handleTimeChange(index, e.target.value)} />
            <button   class="col-1"  onClick={() => this.handleRemoveFilter(index)}>x</button>
          </div>
        ))}
        
        <button   class="col-1" onClick={this.handleAddFilter}>+</button>
        <button onClick={this.handleSubmit}>Submit</button>
      </div>
    );
  }
}

export default Search;