function SearchBox({
    value,
    onChange,
    placeholder = "Search..."
}) {
    return (
        <input
            type="text"
            className="form-control w-auto"
            placeholder={placeholder}
            value={value}
            onChange={(e) => onChange(e.target.value)}
        />
    );
}

export default SearchBox;