﻿import React, { useState } from 'react';
import Select from 'react-select'
import { IMeterMeas } from '../type_defs/IMeterMeas';
import { ISchArchMeasPickerProps } from '../type_defs/ISchArchMeasPickerProps';
import { ISchArchMeas } from '../type_defs/ISchArchMeas';
import { MeasDiscriminator } from '../type_defs/MeasDiscriminator';

function SchArchMeasPicker(props: ISchArchMeasPickerProps) {
    const [selSchType, setSelSchType] = useState(null);
    const [selUtil, setSelUtil] = useState(null);

    const onMeasClick = () => {
        const selMeas: ISchArchMeas = {
            discriminator: MeasDiscriminator.schArch,
            schType: selSchType,
            utilName: selUtil
        }
        props.onMeasSelected(selMeas)
    }

    const handleSchTypeChange = (selectedOption: { label: string, value: string }) => {
        setSelSchType(selectedOption.value)
    }

    const handleUtilChange = (selectedOption: { label: string, value: string }) => {
        setSelUtil(selectedOption.value)
    }

    return (
        <>
            <Select
                placeholder="Select Schedule Type..."
                options={props.schTypesList.map((st) => { return { label: st.t, value: st.v } })}
                onChange={handleSchTypeChange}
            ></Select>
            <Select
                placeholder="Select Schedule Utility..."
                options={props.utilNamesList.map((st) => { return { label: st, value: st } })}
                onChange={handleUtilChange}
            ></Select>
            <button onClick={onMeasClick} className={"btn btn-primary btn-sm btn-icon-split"}>
                <span className={"icon text-white-50"}>
                    <i className={"fas fa-plus"}></i>
                </span>
                <span className={"text"}>Add Schedule Archive Measurement</span>
            </button>
        </>
    );
}

export default SchArchMeasPicker;