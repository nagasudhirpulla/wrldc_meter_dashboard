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
            <button onClick={onMeasClick}>Add Meter Measurement</button>
        </>
    );
}

export default MeterMeasPicker;