import React, { useState } from 'react';
import Select from 'react-select'
import { IScadaArchMeasPickerProps } from '../type_defs/IScadaArchMeasPickerProps';
import { IScadaMeas } from '../type_defs/IScadaMeas';

function ScadaArchMeasPicker(props: IScadaArchMeasPickerProps) {
    const [selMeas, setSelMeas] = useState(null);
    const [selMeasType, setSelMeasType] = useState(null);

    const onMeasClick = () => {
        props.onMeasSelected(selMeas)
    }

    const handleChange = (selectedOption: IScadaMeas) => {
        setSelMeas(selectedOption)
    }

    const handleMeasTypeChange = (selectedOption: { value: string, label: string }) => {
        setSelMeasType(selectedOption.value)
        props.onMeasTypeChanged(selectedOption.value);
    }

    return (
        <>
            <Select
                options={props.measList}
                onChange={handleChange}
                getOptionLabel={option => option.description}
                getOptionValue={option => option.measTag} />
            <Select
                options={props.measTypes.map((mt) => { return { label: mt, value: mt } })}
                onChange={handleMeasTypeChange} />
            <button onClick={onMeasClick}>Select</button>
        </>
    );
}

export default MeterMeasPicker;