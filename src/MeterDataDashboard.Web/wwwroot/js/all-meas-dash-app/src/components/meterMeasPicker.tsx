import React, { useState } from 'react';
import Select from 'react-select'
import { IMeterMeas } from '../type_defs/IMeterMeas';
import { IMeterMeasPickerProps } from '../type_defs/IMeterMeasPickerProps';

function MeterMeasPicker(props: IMeterMeasPickerProps) {
    const [selMeas, setSelMeas] = useState(null);

    const onMeasClick = () => {
        props.onMeasSelected(selMeas)
    }

    const handleChange = (selectedOption: IMeterMeas) => {
        setSelMeas(selectedOption)
    }

    return (
        <>
            <Select
                placeholder="Select Meter Measurement..."
                options={props.measList}
                onChange={handleChange}
                getOptionLabel={option => option.description}
                getOptionValue={option => option.locationTag} />
            <button onClick={onMeasClick} className={"btn btn-primary btn-sm btn-icon-split"}>
                <span className={"icon text-white-50"}>
                    <i className={"fas fa-plus"}></i>
                </span>
                <span className={"text"}>Add Meter Measurement</span>
            </button>
        </>
    );
}

export default MeterMeasPicker;