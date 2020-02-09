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
        props.onMeasTypeChanged(selMeasType);
    }

    return (
        <>
            <Select
                placeholder="Select Type..."
                options={props.measTypes.map((mt) => { return { label: mt, value: mt } })}
                onChange={handleMeasTypeChange} />
            <Select
                placeholder="Select SCADA Measurement..."
                options={props.measList}
                onChange={handleChange}
                getOptionLabel={option => option.description}
                getOptionValue={option => option.measTag} />
            <button onClick={onMeasClick} className={"btn btn-primary btn-sm btn-icon-split"}>
                <span className={"icon text-white-50"}>
                    <i className={"fas fa-plus"}></i>
                </span>
                <span className={"text"}>Add SCADA Archive Measurement</span>
            </button>
        </>
    );
}

export default ScadaArchMeasPicker;